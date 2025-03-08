using DotNetEnv;
using Services;
using Services.DTOs;
using Services.Models;
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

                ClearProductForm();

                MessageBox.Show("Product has been created", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadProducts();
                LoadLicenseTemplates();
                return;
            }
            catch (Exception ex)
            {
                // log error
                MessageBox.Show("Could not find products", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        ////////////////////// License Template
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

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                MessageBox.Show("An error occured while loading License Templates", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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

        private void ClearProductForm()
        {
            txtName.Text = string.Empty;
            txtDisplayName.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }
    }
}
