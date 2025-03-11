using Cryptlex;
using DotNetEnv;
using Services;
using Services.Constants;
using Services.DTOs;
using Services.Models;
using Sprache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;
using ServiceType = Services.Constants.Type;

namespace Cryptlex_Licensing_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProductManagementService _productManagementService;
        private readonly LicenseManagementService _licenseManagementService;
        private readonly LoggerService _logger;
        public List<Product> Products { get; set; }
        public List<LicenseTemplate> LicenseTemplates { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Env.Load();

            _productManagementService = new ProductManagementService();
            _licenseManagementService = new LicenseManagementService();

            _logger = new LoggerService();

            Products = new List<Product>();   
            LicenseTemplates = new List<LicenseTemplate>();

            _licenseManagementService.SetProductFiles();            

            LoadProducts();
            LoadLicenseTemplates();            
        }

        private async void LoadProducts()
        {
            try
            {
                Products.Clear();
                Product[] products = await _productManagementService.GetProductsAsync();

                if(products == null || products.Length == 0)
                {                    
                    MessageBox.Show("Could not find products.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Products.AddRange(products);
                DataContext = null;
                DataContext = this;

                BindProducts();

                return; 
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while loading products.");
                MessageBox.Show("An error occured while loading products.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        private async void CreateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtName.Text == null || txtName.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Product Name.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtName.Focus();
                    return;
                }

                if (txtDisplayName.Text == null || txtDisplayName.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Product Display Name.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtDisplayName.Focus();
                    return;
                }

                if (txtDescription.Text == null || txtDescription.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Product Description.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtDescription.Focus();
                    return;
                }

                if (cmbLicenseTemplate.SelectedValue == null || cmbLicenseTemplate.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select a License Template.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbLicenseTemplate.Focus();
                    return;
                }

                CreateProductDto productDto = new CreateProductDto();

                productDto.Name = txtName.Text;
                productDto.DisplayName = txtDisplayName.Text;
                productDto.Description = txtDescription.Text;
                productDto.LicenseTemplateId = Guid.Parse((string)cmbLicenseTemplate.SelectedValue);

                var isCreated = await _productManagementService.CreateProductAsync(productDto);

                if (!isCreated)
                {
                    MessageBox.Show("Could not create product.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ClearTextBoxText(new TextBox[] { txtName, txtDisplayName, txtDescription });

                MessageBox.Show("Product has been created.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadProducts();
                LoadLicenseTemplates();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while creating Product.");
                MessageBox.Show("An error occured while creating Product.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void DeleteProduct_Click(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBoxResult.No;

                if (e.AddedItems.Count > 0)
                {
                    var selectedProduct = e.AddedItems[0] as Product;
                    result = MessageBox.Show(
                    $"Do you want to delete {selectedProduct.DisplayName}?",
                    "Delete Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                    if(result == MessageBoxResult.Yes)
                    {
                        bool isDeleted = await _productManagementService.DeleteProductAsync(selectedProduct.Id);

                        if (!isDeleted)
                        {
                            MessageBox.Show("Could not delete product.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        LoadProducts();
                    }
                }

                ListBox listBox = sender as ListBox;
                listBox.SelectedItem = null;               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while deleting product.");
                MessageBox.Show("An error occured while deleting product.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);                
            }
        }
        
        private async void LoadLicenseTemplates()
        {
            try
            {
                LicenseTemplates.Clear();
                LicenseTemplate[] licenseTemplates = await _licenseManagementService.GetAllLicenseTemplatesAsync();

                if (licenseTemplates == null || licenseTemplates.Length == 0)
                {
                    MessageBox.Show("Could not find License Templates.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                LicenseTemplates.AddRange(licenseTemplates);
                DataContext = null;
                DataContext = this;

                BindLicenseTemplates();

                LoadComboBoxOptions(FingerprintMatchingStrategy.AllValues, cmbFingerprintMatchingStrategy);
                LoadComboBoxOptions(ExpirationStrategy.AllValues, cmbExpirationStrategy);
                LoadComboBoxOptions(ServiceType.AllValues, cmbType);

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while loading License Templates.");
                MessageBox.Show("An error occured while loading License Templates.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void CreateLicenseTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ltName.Text == null || ltName.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Name.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    ltName.Focus();
                    return;
                }

                if (cmbFingerprintMatchingStrategy.SelectedValue == null || cmbFingerprintMatchingStrategy.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Fingerprint Matching Strategy.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbFingerprintMatchingStrategy.Focus();
                    return;
                }

                if (cmbExpirationStrategy.SelectedValue == null || cmbExpirationStrategy.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Expiration Strategy.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbExpirationStrategy.Focus();
                    return;
                }

                if (cmbType.SelectedValue == null || cmbType.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Type.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbType.Focus();
                    return;
                }

                CreateLicenseTemplateDto licenseTemplateDto = new CreateLicenseTemplateDto();

                licenseTemplateDto.Name = ltName.Text;
                licenseTemplateDto.FingerprintMatchingStrategy = (string)cmbFingerprintMatchingStrategy.SelectedValue;
                licenseTemplateDto.AllowVmActivation = allowVmActivationCheckBox.IsChecked;
                licenseTemplateDto.AllowContainerActivation = allowContainerActivationCheckBox.IsChecked;
                licenseTemplateDto.UserLocked = userLockedCheckBox.IsChecked;
                licenseTemplateDto.DisableGeoLocation = disableGeoLocationCheckBox.IsChecked;
                licenseTemplateDto.Validity = long.Parse(validity.Text);
                licenseTemplateDto.ExpirationStrategy = (string)cmbExpirationStrategy.SelectedValue;
                licenseTemplateDto.AllowedActivations = long.Parse(allowedActivations.Text);
                licenseTemplateDto.AllowedDeactivations = long.Parse(allowedDeactivations.Text);
                licenseTemplateDto.AllowClientLeaseDuration = allowClientLeaseDurationCheckBox.IsChecked;
                licenseTemplateDto.ServerSyncGracePeriod = long.Parse(serverSyncGracePeriod.Text);
                licenseTemplateDto.ServerSyncInterval = long.Parse(serverSyncInterval.Text);
                licenseTemplateDto.AllowedClockOffset = long.Parse(allowedClockOffset.Text);
                licenseTemplateDto.ExpiringSoonEventOffset = long.Parse(expiringSoonEventOffset.Text);
                licenseTemplateDto.Type = (string)cmbType.SelectedValue;

                bool isCreated = await _licenseManagementService.CreateLicenseTemplateAsync(licenseTemplateDto);

                if (!isCreated)
                {
                    MessageBox.Show("Could not create License Template.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                ClearTextBoxText(new TextBox[] { ltName, allowedActivations, allowedDeactivations,  serverSyncGracePeriod, serverSyncInterval, allowedClockOffset, expiringSoonEventOffset, validity });

                MessageBox.Show("License Template has been created.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadProducts();
                LoadLicenseTemplates();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while creating a License Template.");
                MessageBox.Show("An error occured while creating a License Template.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void DeleteLicenseTemplate_Click(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBoxResult.No;

                if (e.AddedItems.Count > 0)
                {
                    var selectedLicenseTemplate = e.AddedItems[0] as LicenseTemplate;
                    result = MessageBox.Show(
                    $"Do you want to delete {selectedLicenseTemplate.Name}?",
                    "Delete Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        bool isDeleted = await _licenseManagementService.DeletelicenseTemplateAsync(selectedLicenseTemplate.Id);

                        if (!isDeleted)
                        {
                            MessageBox.Show("Could not delete License Template.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        LoadLicenseTemplates();
                    }
                }

                ListBox listBox = sender as ListBox;
                listBox.SelectedItem = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while deleting License Template.");
                MessageBox.Show("An error occured while deleting License Template.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CreateLicenseKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbProducts.SelectedValue == null || cmbProducts.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select a Product.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbProducts.Focus();
                    return;
                }

                CreateLicenseKeyDto createLicenseKeyDto = new CreateLicenseKeyDto();

                createLicenseKeyDto.ProductId = Guid.Parse((string)cmbProducts.SelectedValue);

                var license = await _licenseManagementService.GenerateLicenseKeyAsync(createLicenseKeyDto);

                if (license is null)
                {
                    MessageBox.Show("Could not generate License Key.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                txtGeneratedLicenseKey.Text = license.Key;

                MessageBox.Show("License key have been successfully generated. Copy it and click 'Clear' when done.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while Generating License Key.");
                MessageBox.Show("An error occured while Generating License Key.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void activateLicenseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string[] productIds = _licenseManagementService.GetProductIds();

                if(productIds == null || productIds.Length == 0)
                {
                    MessageBox.Show("Activation failed, no Product Id found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    return;
                }

                int status = -1;

                foreach (var productId in productIds)
                {
                    status = _licenseManagementService.ActivateLicense(txtActivatingLicenseKey.Text, productId);

                    if(status > -1)
                    {
                        break;
                    }
                }

                if (status == LexStatusCodes.LA_OK || status == LexStatusCodes.LA_EXPIRED || status == LexStatusCodes.LA_SUSPENDED)
                {
                    MessageBox.Show("Activation successful.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    txtActivatingLicenseKey.Text = "";
                }
                else
                {                   
                    MessageBox.Show("Activation failed.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (LexActivatorException ex)
            {
                _logger.LogError(ex, "An error occured while Activating License.");
                MessageBox.Show("An error occured while Activating License.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearGeneratedLicenseKey_Click(object sender, RoutedEventArgs e)
        {
            txtGeneratedLicenseKey.Text = "";
        }
        private void ClearLicenseKey_Click(object sender, RoutedEventArgs e)
        {
            txtActivatingLicenseKey.Text = "";
        }

        private void BindProducts()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Text");
            dt.Columns.Add("Value");
            dt.Rows.Add("--SELECT--", null);
            
            foreach (var product in Products)
            {
                dt.Rows.Add(product.Name, product.Id);
            }
            
            cmbProducts.ItemsSource = dt.DefaultView;
            cmbProducts.DisplayMemberPath = "Text";
            cmbProducts.SelectedValuePath = "Value";
            cmbProducts.SelectedIndex = 0;
        }

        private void BindLicenseTemplates()
        {
            DataTable dtLicenseTemplates = new DataTable();
            dtLicenseTemplates.Columns.Add("Text");
            dtLicenseTemplates.Columns.Add("Value");
            dtLicenseTemplates.Rows.Add("--SELECT--", null);
            
            foreach (var licenseTemplate in LicenseTemplates)
            {
                dtLicenseTemplates.Rows.Add(licenseTemplate.Name, licenseTemplate.Id);
            }
            
            cmbLicenseTemplate.ItemsSource = dtLicenseTemplates.DefaultView;
            cmbLicenseTemplate.DisplayMemberPath = "Text";
            cmbLicenseTemplate.SelectedValuePath = "Value";
            cmbLicenseTemplate.SelectedIndex = 0;
        }

        private void LoadComboBoxOptions(string[] options, ComboBox comboBox)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Value");
            dt.Rows.Add("--SELECT--");
            foreach (var option in options)
            {
                dt.Rows.Add(option);
            }
            
            comboBox.ItemsSource = dt.DefaultView;
            comboBox.DisplayMemberPath = "Value";
            comboBox.SelectedValuePath = "Value";
            comboBox.SelectedIndex = 0;
        }

        private void ClearTextBoxText(TextBox[] textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                textBox.Text = string.Empty;
            }
        }

        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
