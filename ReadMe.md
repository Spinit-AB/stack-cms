# Spinit Stack CMS

## Installation

### Donwload

1. Clone the reposirtory https://github.com/Spinit-AB/stack-cms.git

2. Copy this complete folder to a location of your choice. Exclude the .git-folder though, or remove it from the location you copied everything to.

3. Now, before you continue, close this file and open up the copy you just created instead. :)

### Project Setup

1. Open up Powershell and navigate to your chosen location
Execute the configure script with name and port parameters. The port will be the port on which your local iis express should host this site  
`.\configure.ps1 -name 'ANameOfYourChoice'`

2. Create a empty database (Umbraco supports SQL server, Azure SQL, MySQL or a custom connectionstring)

3. Open the solution-file in Visual Studio and launch the project (if npm will kick in on startup this can take a while and it can seem like Visual Studio is hanging, be patient :) )

4. When you launch the site Umbraco will greet you with a Installation guide. Fill in credentials you want for your account. Be careful as the password is displayed in clear text. Later the password will be visable in Visual Studio settings so dont use your own password.

Click "Customize"

!!! Important !!! Dont click "Install" here.

5. Select your database type and click "Continue"

6. Enter your database credentials and click "Continue"

7. Click "No Thanks! I dont want a starter kit"

8. If everything goes as planned you should now be redirected to Umbraco CMS editing mode, eg http://localhost:12345/umbraco#/umbraco

8. 	From Visual Studio. Go to Tools->"Extensions and Updates"
	Find and install "Umbraco ModelsBuilder Custom Tool"
	
	Restart Visual Studio.

9. 	Open Visual Studio again and go to Tools->Options->Umbraco->ModelsBuilder Options
	This should appear after the extension install in step 8.
	
    Update the settings:

    SiteUrl (eg http://localhost:12345)  
    User Name (its the mail you entered at when installed Umbraco)
    User Password (And the password you entered when Installing Umbraco)

    This will give you possibility to generate your Umbraco models to backend so you can use them  as strictly classes from code.

10. Now your site is installed! Please check the other setups to get everything up and running.

### Npm setup
1. If your Visual Studio didnt run npm for you we can have to make it manually. You will notice if your  <project-root>/node_modules are populated or not. If not. Please proceed. 
2. Install [Node.js](https://nodejs.org/en/). If you havent installed npm already.
2. Open command prompt
3. Change directory to <project-root>/Spinit.Stack.CMS/
4. Execute `npm install`

    If you want a new package run  

    `npm install package-name --save` Use this if you want the main files of the package to automatically inject into <project-root>/Spinit.Stack.CMS/Views/Shared/_Layout.cshtml 

    or run  

    `npm install package-name --save-dev` This will install the package so your fellow users can take a share of it. But you will have to manually include the packages to <project-root>/Spinit.Stack.CMS/Views/Shared/_Layout.cshtml 
    You can always check what mainfiles gulp wants to inject by checking the  in the node_modules/<package-name>/package.json and check what files are showing in the "main" section.

    If you're installing a package that is not included automatically because in the npm main files you need to add the path to the dist-js variable "manualNodeDependenciesJs" in the gulp.config.js

### Gulp taskrunner
1. Open command prompt
2. Change directory to <project-root>/Spinit.Stack.CMS/
3. Install gulp `npm install --g gulp`. If you havent installed gulp already.
4. Gulp commands to execute:

    `gulp` or `gulp watch`

    Injects created or npm-packages (not devDependencies) js and css files to <project-root>/Spinit.Stack.CMS/Views/Shared/_Layout.cshtml

    `gulp build`
    
    This will create a release with minified js and css into <project-root>/Spinit.Stack.CMS/dist and inject the minified versions into <project-root>/Spinit.Stack.CMS/Views/Shared/_Layout.cshtml 

### Webpack
1. Open command prompt
2. Change directory to <project-root>/Spinit.Stack.CMS/
3. Run ```webpack``` to build your files or use ```webpack watch``` to notice changes

Release
Build, minimize and uglify with
```webpack -p```

### TeamCity
1. Create a project
2. Go to "General settings" of the project
3. Click "Create build configuration"->"Manually"
4. Enter a name of the build eg "Release"
5. Click "Based on template" and select "Spinit.CMS.Webpack"
6. Add your repo to "VCS Root"
7. Push a change to the master branch
8. The TeamCity should now build and push a package named "Spinit.Stack.CMS.1.0.0.0" to https://spinit.octopus.se
### Personal Debug Connection String
If you want custom conenction strings that dosent need to be committed into the repository you can activate the debug connection string.

1. Make sure you run the initial Umbraco setup.

2. Create a file named "connectionStrings.Debug.config" in Spinit.Stack.CMS/

3. Go to Web.Config and uncomment
```
<!--<connectionStrings configSource="connectionStrings.Debug.config"></connectionStrings>-->
 ```
and make sure to remove the other <connectionString></connectionString> node

4. Add your custom connectionString in "connectionStrings.Debug.config", 
 for example:
```
<connectionStrings>
  <remove name="umbracoDbDSN" />
  <add name="umbracoDbDSN" connectionString="Server=.\SQL2016;Database=StackCMS;Integrated Security=true" providerName="System.Data.SqlClient" />
</connectionStrings>
```

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

### Create a new Document Type

1. Go to Umbraco edit mode http://localhost:12345/umbraco

2. Go to Settings and open "Document Types" folder.

3. Hover over a node (if you want to inherite variables from eg "Home" you can create your new document type under this node) and click the three dots ***

4. Give your document type a name (eg "Article Page") and and click "Save"

5. Make sure that your document type is allowed to be listed under the parent of your new document type. From Settings click the document type of the parent and select "Permissions" in the right corner. Click "Add child" and select your createed document type (eg Article Page)

6. Your document type shoould be generated in Spinit.Stack.CMS/GeneretedModels/ModelsBuilder.cs
    If its not generating you can right click on the "ModelsBuilder.cs" and select "Run custom tool" and it should appear. 
    If not, check your settings in Tools->Options->Umbraco->ModelsBuilder Options

7. Create a new folder (eg ArticlePage) under Spinit.Stack.CMS/Features/

8. Create a new model class (eg ArticlePageModel.cs) and make it inherit from the generated ArticlePage

    ```
    public class ArticlePageModel : GeneratedModels.ArticlePage
        {
            public ArticlePageModel(IPublishedContent content) : base(content)
            {
            }
        }
    ```
    Here you can add values you want to send to your view from the backend. It also contains the variables that are defined in Umbraco, those will also be available in the view.

9. Create a new controller class (eg ArticlePageController). We need to setup the controller so Umbraco will recognize it with RenderMvcController. You can just copy the code below

    ```
    public class ArticlePageController : RenderMvcController
        {
            public override ActionResult Index(RenderModel model)
            {
                var articlePageModel = new ArticlePageModel(model.Content)
                {
                    //Here you can set your custom model variables that are not defined in Umbraco
                };

                return base.CurrentTemplate(articlePageModel);
            }
        }
    ```

10. Umbraco should have created a View page for you. Go to Spinit.Stack.CMS/Views in Visual Studio Solution Explorer and click the refresh button and it should appear. Right click and include the view in the project. The view needs to inherits from UmbracoViewPage with your created model. You can also access all your model variables from @Model

    ```
    @inherits UmbracoViewPage<Brottsportalen.Web.Features.ArticlePage.ArticlePageModel>
    @{
        Layout = "Shared/_Layout.cshtml";
    }

    <h1>@Model.PageTitle</h1>   
    ```

11. Build your project and go to Umbraco edit mode http://localhost:12345/umbraco

12. Go to content and hover over your node (eg Home) and click the three dots ***. Select your item (eg Article Page). Give it a name and click "Save and Publish"

13. Visit the created page. Select the page Properties->Link to document to find the friendly link to it.

14. If everything worked you now have a new document type! 

    You can go  back to Settings->"Document Types" and add specific document type properties by selecting the document type and click "Add proprty"


## log4net
- To use Umbracos LogHelper:
    ```
    LogHelper.Info(GetType(), "Test log");
    ```
- Logs are stored default in <project-root>/Spinit.Stack.CMS/App_Data/Logs
- Log4net Config 
<project-root>/Spinit.Stack.CMS/config/log4net.config