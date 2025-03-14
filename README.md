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
   - <span style="color:red">If you encounter the following prompt:
      ```
      File 'ILLink\ILLink.Descriptors.LibraryBuild.xml' already exists in project 'Cryptlex-Licensing-Application'. Do you want to overwrite it? [Y] Yes  [A] Yes to All  [N] No  [L] No to All  [?] Help (default is "N"):
      ```
      *Select A (Yes to All) to automatically overwrite all existing files and proceed with the update without further interruptions.*</span>
   
4. To use your Cryptlex access token:
   - Open the **.env** file in the root folder.
   - Replace the value of `ACCESS_TOKEN` with your Cryptlex access token.
      ```
      ACCESS_TOKEN=your_access_token
      ```
5. To add your products for license activation:
   - Place your product's **Product.dat** file in the **Config/ProductDat** folder.
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
     - To create a Product: Fill out the form and click **Create Product**.
     - To delete a Product: Click on the Product name.

   - **Manage Licenses *Tab***
     - To create a License Template: Fill out the form and click **Create License Template**.
     - To delete a License Template: Click on the License Template name.
     - To generate a License key: Select the product for which you want to generate a license key in the **Generate License Key** section and click **Create**.
     - To activate a License key: Paste your license key in the **Activate License Key** section and click **Activate**.