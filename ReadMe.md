# Spinit Stack CMS

## Installation

### Donwload

1. Clone the reposirtory https://github.com/Spinit-AB/stack-cms.git

2. Copy this complete folder to a location of your choice. Exclude the .git-folder though, or remove it from the location you copied everything to.

3. Now, before you continue, close this file and open up the copy you just created instead. :)

### Project Setup

2. Open up Powershell and navigate to your chosen location
Execute the configure script with name and port parameters. The port will be the port on which your local iis express should host this site  
.\configure.ps1 -name 'ANameOfYourChoice'

3. Create a empty database (Umbraco supports SQL server, Azure SQL, MySQL or a custom connectionstring)

4. Open the solution-file in Visual Studio and launch the project (if npm will kick in on startup this can take a while and it can seem like Visual Studio is hanging, be patient :) )

5. When you launch the site Umbraco will greet you with a Installation guide. Fill in credentials for your account and click "Customize"

!!! Important !!! Dont click "Install" here.

6. Select your database type and click "Continue"

7. Enter your database credentials and click "Continue"

8. Click "No Thanks! I dont want a starter kit"

9. If everything goes as planned you should now be redirected to Umbraco CMS editing mode, eg http://localhost:12345/umbraco#/umbraco

10. From Visual Studio. Go to Tools->Options->Umbraco->ModelsBuilder Options
    Update the settings:

    SiteUrl (eg http://localhost:12345)  
    User Name (its the mail you entered at when installed Umbraco)
    User Password (And the password you entered when Installing Umbraco)

    This will automatically generate your Umbraco models to backend so you can strictly use them from code.

11. Now your site is installed! Please check the other setups to get everything up and running.

### Npm setup
1. If your Visual Studio didnt run npm for you we can always make it manually. You will notice if your  <project-root>/node_modules are populated or not. If not. Please proceed. 
2. Install [Node.js](https://nodejs.org/en/). If you havent installed npm already.
2. Open command prompt
3. Change directory to <project-root>/Spinit.Stack.CMS/
4. Execute `npm install`

If you want a new package run  

`npm install package-name --save` Use this if you want the main files of the package to automatically inject into <project-root>/Spinit.Stack.CMS/Views/Shared/_Layout.cshtml 

or run  

`npm install package-name --save-dev` This will install the package so your fellow users can take a share of it. But you will have to manually include the packages to <project-root>/Spinit.Stack.CMS/Views/Shared/_Layout.cshtml 

### Gulp taskrunner
1. Open command prompt
2. Change directory to <project-root>/Spinit.Stack.CMS/
3. Install gulp `npm install --g gulp`. If you havent installed gulp already.
4. Gulp commands to execute:

    `gulp` or `gulp watch`

    Injects created or npm-packages (not devDependencies) js and css files to <project-root>/Spinit.Stack.CMS/Views/Shared/_Layout.cshtml

    `gulp build`
    
    This will create a release with minified js and css into <project-root>/Spinit.Stack.CMS/dist and inject the minified versions into <project-root>/Spinit.Stack.CMS/Views/Shared/_Layout.cshtml 

### Umbraco Page Setup

1. Start by creating the home page. 
    - Hover over the "Content" text in the second left column and click the three dots ***
    - In the Create tab that appears click "Home"
    - Give a name to the page (eg Home) and enter "Page Title" (Eg Welcome to <customername>)
    - Switch to the "Properties tab" and in the dropdown for "Tamplate" select "Home"
    - Click "Save and publish"

2. Create a content page.
    - Hover over the "Home"-page just crated and click the three dots ***
    - Select "Content Page"
    - Give the page a name and a PageTitle
    - Go to "Properties"-taba and select "Content Page" in the "Template"-dropwdown 

3. Go to http://localhost:12345/ 
    You should now have a page with a navbar menu for your two pages

### Create a new pagetype


## log4net
- To use Umbracos LogHelper:
```
LogHelper.Info(GetType(), "Test log");
```
- Logs are stored default in <project-root>/Spinit.Stack.CMS/App_Data/Logs
- Log4net Config 
<project-root>/Spinit.Stack.CMS/config/log4net.config