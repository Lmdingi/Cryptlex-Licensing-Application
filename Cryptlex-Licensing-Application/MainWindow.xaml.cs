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

            _productManagementService = new ProductManagementService(Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            _licenseManagementService = new LicenseManagementService(Environment.GetEnvironmentVariable("ACCESS_TOKEN"));

            _logger = new LoggerService();

            Products = new List<Product>();   
            LicenseTemplates = new List<LicenseTemplate>();

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
                    MessageBox.Show("Could not find products", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Products.AddRange(products);
                DataContext = null;
                DataContext = this;

                return; 
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                MessageBox.Show("An error occured while loading products", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        private async void CreateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtName.Text == null || txtName.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Product Name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtName.Focus();
                    return;
                }

                if (txtDisplayName.Text == null || txtDisplayName.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Product Display Name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtDisplayName.Focus();
                    return;
                }

                if (txtDescription.Text == null || txtDescription.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Product Description", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtDescription.Focus();
                    return;
                }

                if (cmbLicenseTemplate.SelectedValue == null || cmbLicenseTemplate.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select a License Template", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show("Could not create product", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ClearProductForm(new TextBox[] { txtName, txtDisplayName, txtDescription });

                MessageBox.Show("Product has been created", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadProducts();
                LoadLicenseTemplates();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                MessageBox.Show("An error occured while creating Product", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                            MessageBox.Show("Could not delete product", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
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
                _logger.LogError(ex.Message, ex);
                MessageBox.Show("An error occured while deleting product", "Error", MessageBoxButton.OK, MessageBoxImage.Error);                
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
                    MessageBox.Show("Could not find License Templates", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                LicenseTemplates.AddRange(licenseTemplates);
                DataContext = null;
                DataContext = this;

                BindLicenseTemplates();

                loadComboBoxOptions(FingerprintMatchingStrategy.AllValues, cmbFingerprintMatchingStrategy);
                loadComboBoxOptions(ExpirationStrategy.AllValues, cmbExpirationStrategy);
                loadComboBoxOptions(ServiceType.AllValues, cmbType);

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                MessageBox.Show("An error occured while loading License Templates", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void CreateLicenseTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ltName.Text == null || ltName.Text.Trim() == "")
                {
                    MessageBox.Show("Please Enter Name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    ltName.Focus();
                    return;
                }

                if (cmbFingerprintMatchingStrategy.SelectedValue == null || cmbFingerprintMatchingStrategy.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Fingerprint Matching Strategy", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbFingerprintMatchingStrategy.Focus();
                    return;
                }

                if (cmbExpirationStrategy.SelectedValue == null || cmbExpirationStrategy.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Expiration Strategy", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbExpirationStrategy.Focus();
                    return;
                }

                if (cmbType.SelectedValue == null || cmbType.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Type", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show("Could not create License Template", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                ClearProductForm(new TextBox[] { ltName, allowedActivations, allowedDeactivations,  serverSyncGracePeriod, serverSyncInterval, allowedClockOffset, expiringSoonEventOffset, validity });

                MessageBox.Show("License Template has been created", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadProducts();
                LoadLicenseTemplates();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                MessageBox.Show("An error occured while creating a License Template", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                            MessageBox.Show("Could not delete License Template", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
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
                _logger.LogError(ex.Message, ex);
                MessageBox.Show("An error occured while deleting License Template", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindLicenseTemplates()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private void loadComboBoxOptions(string[] options, ComboBox comboBox)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private void ClearProductForm(TextBox[] textBoxes)
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
