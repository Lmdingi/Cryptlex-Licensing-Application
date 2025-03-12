# Cryptlex-Licensing-Application

## Overview
This application is a WPF project built on .NET 4.7, integrating the Cryptlex API for managing products and licenses. The application allows users to create and manage products, generate license keys, and activate licenses.

#### Features
- Create and manage Products in Cryptlex.
- Create and manage License Templates in Cryptlex.
- Generate and activate License Keys.

#### Installation Instructions
**Prerequisites**
- .NET Framework 4.7 installed
- Cryptlex account
- Visual Studio

**Steps**
1. Clone this [repository](https://github.com/Lmdingi/Cryptlex-Licensing-Application):
   ``` bash
    git clone https://github.com/Lmdingi/Cryptlex-Licensing-Application
   ```
2. Open the project in Visual Studio.
3. Run the following command in the **Package Manager Console** to update all NuGet packages:
   ```powershell
   Update-Package
   ```
   - If you encounter the following prompt:
      ```
      File 'ILLink\ILLink.Descriptors.LibraryBuild.xml' already exists in project 'Cryptlex-Licensing-Application'. Do you want to overwrite it? [Y] Yes  [A] Yes to All  [N] No  [L] No to All  [?] Help (default is "N"):
      ```
      *Select A (Yes to All) to automatically overwrite all existing files and proceed with the update without further interruptions.*
   
4. To use your Cryptlex access token:
   - Open the **.env** file in the root folder.
   - Replace the value of `ACCESS_TOKEN` with your Cryptlex access token.
      ```
      ACCESS_TOKEN=your_access_token
      ```
5. To add your products for license activation:
   - Place your product's **Product.dat** file in the **Config/Productdat** folder.
   - Place your product's ID in the **ProductId.json** file inside the **Config/ProductId** folder. 
      ``` json
      {
         "ProductIds": [
            "your_product_id",
         ]
      }
      ```

#### Usage Instructions

Launch the application.
   - The Product list and the License Template list  will load if you are connected.
   - **Manage Products *Tab***
     - Click on the Product name to delete it.
     - Fill out the form and click **Create** to add a new product.

   - **Manage Licenses *Tab***
     - Click on the License Template name to delete it.
     - Fill out the form and click **Create** to add a new License Template.
     - Select the product for which you want to generate a license key in the **Generate License Key** section and click **Create**.
     - Paste your license key in the **Activate License Key** section and click **Activate**.