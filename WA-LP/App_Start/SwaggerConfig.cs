using System.Web.Http;
using WebActivatorEx;
using WA_LP;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using SwaggerDocument = Swashbuckle.Swagger.SwaggerDocument;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using SharedModel.Models.Services.Banks.Galicia;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WA_LP
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SchemaId(x => x.FullName);
                        //c.ApiKey("apikey")
                        //    .Name("Api-Key")
                        //    .In("header");
                        // By default, the service root url is inferred from the request used to access the docs.
                        // However, there may be situations (e.g. proxy and load-balanced environments) where this does not
                        // resolve correctly. You can workaround this by providing your own code to determine the root URL.
                        //
                        //c.RootUrl(req => GetRootUrlFromAppConfig());

                        // If schemes are not explicitly provided in a Swagger 2.0 document, then the scheme used to access
                        // the docs is taken as the default. If your API supports multiple schemes and you want to be explicit
                        // about them, you can use the "Schemes" option as shown below.
                        //
                        //c.Schemes(new[] { "http", "https" });

                        // Use "SingleApiVersion" to describe a single version API. Swagger 2.0 includes an "Info" object to
                        // hold additional metadata for an API. Version and title are required but you can also provide
                        // additional fields by chaining methods off SingleApiVersion.
                        //
                        c.SingleApiVersion("v1", "WA_LP");

                        // If you want the output Swagger docs to be indented properly, enable the "PrettyPrint" option.
                        //
                        c.PrettyPrint();

                        // If your API has multiple versions, use "MultipleApiVersions" instead of "SingleApiVersion".
                        // In this case, you must provide a lambda that tells Swashbuckle which actions should be
                        // included in the docs for a given API version. Like "SingleApiVersion", each call to "Version"
                        // returns an "Info" builder so you can provide additional metadata per API version.
                        //
                        //c.MultipleApiVersions(
                        //    (apiDesc, targetApiVersion) => ResolveVersionSupportByRouteConstraint(apiDesc, targetApiVersion),
                        //    (vc) =>
                        //    {
                        //        vc.Version("v2", "Swashbuckle Dummy API V2");
                        //        vc.Version("v1", "Swashbuckle Dummy API V1");
                        //    });

                        // You can use "BasicAuth", "ApiKey" or "OAuth2" options to describe security schemes for the API.
                        // See https://github.com/swagger-api/swagger-spec/blob/master/versions/2.0.md for more details.
                        // NOTE: These only define the schemes and need to be coupled with a corresponding "security" property
                        // at the document or operation level to indicate which schemes are required for an operation. To do this,
                        // you'll need to implement a custom IDocumentFilter and/or IOperationFilter to set these properties
                        // according to your specific authorization implementation
                        //
                        //c.BasicAuth("basic")
                        //    .Description("Basic HTTP Authentication");
                        //
                        // NOTE: You must also configure 'EnableApiKeySupport' below in the SwaggerUI section
                        //c.ApiKey("apiKey")
                        //    .Description("API Key Authentication")
                        //    .Name("apiKey")
                        //    .In("header");
                        //
                        //c.OAuth2("oauth2")
                        //    .Description("OAuth2 Implicit Grant")
                        //    .Flow("implicit")
                        //    .AuthorizationUrl("http://petstore.swagger.wordnik.com/api/oauth/dialog")
                        //    //.TokenUrl("https://tempuri.org/token")
                        //    .Scopes(scopes =>
                        //    {
                        //        scopes.Add("read", "Read access to protected resources");
                        //        scopes.Add("write", "Write access to protected resources");
                        //    });

                        // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                        //c.IgnoreObsoleteActions();

                        // Each operation be assigned one or more tags which are then used by consumers for various reasons.
                        // For example, the swagger-ui groups operations according to the first tag of each operation.
                        // By default, this will be controller name but you can use the "GroupActionsBy" option to
                        // override with any value.
                        //
                        //c.GroupActionsBy(apiDesc => apiDesc.HttpMethod.ToString());

                        // You can also specify a custom sort order for groups (as defined by "GroupActionsBy") to dictate
                        // the order in which operations are listed. For example, if the default grouping is in place
                        // (controller name) and you specify a descending alphabetic sort order, then actions from a
                        // ProductsController will be listed before those from a CustomersController. This is typically
                        // used to customize the order of groupings in the swagger-ui.
                        //
                        //c.OrderActionGroupsBy(new DescendingAlphabeticComparer());

                        // If you annotate Controllers and API Types with
                        // Xml comments (http://msdn.microsoft.com/en-us/library/b2s063f7(v=vs.110).aspx), you can incorporate
                        // those comments into the generated docs and UI. You can enable this by providing the path to one or
                        // more Xml comment files.
                        //
                        //c.IncludeXmlComments(GetXmlCommentsPath());
                        c.IncludeXmlComments(string.Format(@"{0}\bin\WA-LP.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                        // Swashbuckle makes a best attempt at generating Swagger compliant JSON schemas for the various types
                        // exposed in your API. However, there may be occasions when more control of the output is needed.
                        // This is supported through the "MapType" and "SchemaFilter" options:
                        //
                        // Use the "MapType" option to override the Schema generation for a specific type.
                        // It should be noted that the resulting Schema will be placed "inline" for any applicable Operations.
                        // While Swagger 2.0 supports inline definitions for "all" Schema types, the swagger-ui tool does not.
                        // It expects "complex" Schemas to be defined separately and referenced. For this reason, you should only
                        // use the "MapType" option when the resulting Schema is a primitive or array type. If you need to alter a
                        // complex Schema, use a Schema filter.
                        //
                        //c.MapType<ProductType>(() => new Schema { type = "integer", format = "int32" });

                        // If you want to post-modify "complex" Schemas once they've been generated, across the board or for a
                        // specific type, you can wire up one or more Schema filters.
                        //
                        //c.SchemaFilter<ApplySchemaVendorExtensions>();
                        //c.SchemaFilter<PolymorphismSchemaFilter<PayOut.Update.Request>>();

                        // In a Swagger 2.0 document, complex types are typically declared globally and referenced by unique
                        // Schema Id. By default, Swashbuckle does NOT use the full type name in Schema Ids. In most cases, this
                        // works well because it prevents the "implementation detail" of type namespaces from leaking into your
                        // Swagger docs and UI. However, if you have multiple types in your API with the same class name, you'll
                        // need to opt out of this behavior to avoid Schema Id conflicts.
                        //
                        c.UseFullTypeNameInSchemaIds();

                        // Alternatively, you can provide your own custom strategy for inferring SchemaId's for
                        // describing "complex" types in your API.
                        //
                        //c.SchemaId(t => t.FullName.Contains('`') ? t.FullName.Substring(0, t.FullName.IndexOf('`')) : t.FullName);

                        // Set this flag to omit schema property descriptions for any type properties decorated with the
                        // Obsolete attribute
                        //c.IgnoreObsoleteProperties();

                        // In accordance with the built in JsonSerializer, Swashbuckle will, by default, describe enums as integers.
                        // You can change the serializer behavior by configuring the StringToEnumConverter globally or for a given
                        // enum type. Swashbuckle will honor this change out-of-the-box. However, if you use a different
                        // approach to serialize enums as strings, you can also force Swashbuckle to describe them as strings.
                        //
                        c.DescribeAllEnumsAsStrings();

                        // Similar to Schema filters, Swashbuckle also supports Operation and Document filters:
                        //
                        // Post-modify Operation descriptions once they've been generated by wiring up one or more
                        // Operation filters.
                        //

                        c.OperationFilter<AddHeaderParameter>();
                        c.OperationFilter<Consumes>();
                        //c.OperationFilter<SwaggerParameterAttributeFilter>();
                        //c.OperationFilter<AddDefaultResponse>();
                        //
                        // If you've defined an OAuth2 flow as described above, you could use a custom filter
                        // to inspect some attribute on each action and infer which (if any) OAuth2 scopes are required
                        // to execute the operation
                        //
                        //c.OperationFilter<AssignOAuth2SecurityRequirements>();

                        // Post-modify the entire Swagger document by wiring up one or more Document filters.
                        // This gives full control to modify the final SwaggerDocument. You should have a good understanding of
                        // the Swagger 2.0 spec. - https://github.com/swagger-api/swagger-spec/blob/master/versions/2.0.md
                        // before using this option.
                        //
                        c.DocumentFilter<XLogoDocumentFilter>();
                        //c.DocumentFilter<SampleCode>();
                        c.DocumentFilter<DefinitionsCode>();

                        //c.DocumentFilter<PolymorphismDocumentFilter<PayOut.Update.Request>>();

                        // In contrast to WebApi, Swagger 2.0 does not include the query string component when mapping a URL
                        // to an action. As a result, Swashbuckle will raise an exception if it encounters multiple actions
                        // with the same path (sans query string) and HTTP method. You can workaround this by providing a
                        // custom strategy to pick a winner or merge the descriptions for the purposes of the Swagger docs
                        //
                        //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                        // Wrap the default SwaggerGenerator with additional behavior (e.g. caching) or provide an
                        // alternative implementation for ISwaggerProvider with the CustomProvider option.
                        //
                        //c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));
                    })
                .EnableSwaggerUi(c =>
                    {
                        // Use the "DocumentTitle" option to change the Document title.
                        // Very helpful when you have multiple Swagger pages open, to tell them apart.
                        //
                        c.DocumentTitle("My Swagger UI");

                        // Use the "InjectStylesheet" option to enrich the UI with one or more additional CSS stylesheets.
                        // The file must be included in your project as an "Embedded Resource", and then the resource's
                        // "Logical Name" is passed to the method as shown below.
                        //
                        //c.InjectStylesheet(containingAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testStyles1.css");

                        // Use the "InjectJavaScript" option to invoke one or more custom JavaScripts after the swagger-ui
                        // has loaded. The file must be included in your project as an "Embedded Resource", and then the resource's
                        // "Logical Name" is passed to the method as shown above.
                        //
                        //c.InjectJavaScript(thisAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testScript1.js");

                        // The swagger-ui renders boolean data types as a dropdown. By default, it provides "true" and "false"
                        // strings as the possible choices. You can use this option to change these to something else,
                        // for example 0 and 1.
                        //
                        //c.BooleanValues(new[] { "0", "1" });

                        // By default, swagger-ui will validate specs against swagger.io's online validator and display the result
                        // in a badge at the bottom of the page. Use these options to set a different validator URL or to disable the
                        // feature entirely.
                        //c.SetValidatorUrl("http://localhost/validator");
                        //c.DisableValidator();

                        // Use this option to control how the Operation listing is displayed.
                        // It can be set to "None" (default), "List" (shows operations for each resource),
                        // or "Full" (fully expanded: shows operations and their details).
                        //
                        //c.DocExpansion(DocExpansion.List);

                        // Specify which HTTP operations will have the 'Try it out!' option. An empty paramter list disables
                        // it for all operations.
                        //
                        //c.SupportedSubmitMethods("GET", "HEAD");

                        // Use the CustomAsset option to provide your own version of assets used in the swagger-ui.
                        // It's typically used to instruct Swashbuckle to return your version instead of the default
                        // when a request is made for "index.html". As with all custom content, the file must be included
                        // in your project as an "Embedded Resource", and then the resource's "Logical Name" is passed to
                        // the method as shown below.
                        //
                        //c.CustomAsset("index", containingAssembly, "YourWebApiProject.SwaggerExtensions.index.html");

                        // If your API has multiple versions and you've applied the MultipleApiVersions setting
                        // as described above, you can also enable a select box in the swagger-ui, that displays
                        // a discovery URL for each version. This provides a convenient way for users to browse documentation
                        // for different API versions.
                        //
                        //c.EnableDiscoveryUrlSelector();

                        // If your API supports the OAuth2 Implicit flow, and you've described it correctly, according to
                        // the Swagger 2.0 specification, you can enable UI support as shown below.
                        //
                        //c.EnableOAuth2Support(
                        //    clientId: "test-client-id",
                        //    clientSecret: null,
                        //    realm: "test-realm",
                        //    appName: "Swagger UI"
                        //    //additionalQueryStringParams: new Dictionary<string, string>() { { "foo", "bar" } }
                        //);

                        // If your API supports ApiKey, you can override the default values.
                        // "apiKeyIn" can either be "query" or "header"
                        //
                        //c.EnableApiKeySupport("Api-Key", "header");
                    });


        }


        //IDocumentFilter

        public class XLogoDocumentFilter : IDocumentFilter
        {
            public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
            {
                {
                    if (!swaggerDoc.info.vendorExtensions.ContainsKey("x-logo"))
                    {
                        swaggerDoc.info.vendorExtensions.Add("x-logo", new
                        {
                            url = "https://www.localpayment.com/wp-content/uploads/2019/04/lp_logo_lineal.png",
                            altText = "LocalPayment logo",
                        });
                    }

                }
            }
        }

        public class SampleCode : IDocumentFilter
        {
            public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
            {
                PathItem path = swaggerDoc.paths.Where(x => x.Key.Contains("/v2/payouts/payout")).First().Value;
                path.post.vendorExtensions.Add("x-code-samples", new[]
                {
                    new
                    {
                        lang= "200 OK Argentina",
                        source= "[\n{\n\"beneficiary_cuit\": \"stringstrin\",\n\"beneficiary_name\": \"string\",\n\"bank_account_type\": \"s\",\n\"bank_cbu\": \"stringstringstringstri\",\n\"amount\": 100,\n\"beneficiary_softd\": \"string\",\n\"site_transaction_id\": \"string\",\n\"concept_code\": 0,\n\"currency\": \"string\",\n\"payout_date\": \"string\",\n\"email\": \"string\",\n\"address\": \"string\",\n\"birth_date\": \"string\",\n\"country\": \"string\",\n\"city\": \"string\",\n\"annotation\": \"string\",\n\"subclient_code\": \"string\",\n\"ErrorRow\": {}\n}\n]\n",
                    }
                });
            }
        }

        public class DefinitionsCode : IDocumentFilter
        {
            public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
            {
                if (swaggerDoc.definitions != null)
                {
                    swaggerDoc.definitions.Add("country_code", new Schema
                    {
                        description = "description of country code"
                    });

                    //swaggerDoc.definitions.Add("ARG", swaggerDoc.definitions["SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Request"]);
                    //var  definition = swaggerDoc.definitions.Where(x => x.Key.Contains("ARG")).First().Value;
                    //definition.allOf.Add(new Schema
                    //{
                    //   @ref = "#/definitions/country_code"
                    //});

                    //swaggerDoc.definitions.Add("COL", swaggerDoc.definitions["SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response"]);


                }

                if (swaggerDoc.paths != null)
                {
                    //swaggerDoc.paths["/v2/payouts/payout"].get.responses["200"].schema.@ref = "#/definitions/ARG";
                    //swaggerDoc.paths["/v2/payouts/payout"].post.parameters.Add(DefinitionParam("#/definitions/ARG" ,"body","body",true,"This is for the Argentina model, but you can choose another model that is specified at the top"));
                    //swaggerDoc.paths["/v2/payouts/payout"].post.parameters.Add(DefinitionParam("#/definitions/COL", "body","body",true,"This is for the Colombia model, but you can choose another model that is specified at the top"));
                }
            }
            public Parameter DefinitionParam(string referency, string name, string inParam, bool required = true, string description = "")
            {
                return new Parameter
                {
                    name = name,
                    @in = inParam,
                    required = required,
                    description = description,
                    schema = new Schema
                    {
                        @ref = referency
                    }
                };
            }
        }
        

        //IOperationFilter

        private class AddHeaderParameter : IOperationFilter
        {
            public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
            {
                if (operation.parameters == null)
                    operation.parameters = new List<Parameter>();

                operation.parameters.Add(HeaderParam("Content-Type", "application/json", true, "Content Type"));
                switch (operation.operationId)
                {
                    //AuthenticationController
                    case "Authentication_Token":
                        operation.parameters.Add(CostumerId);
                        operation.parameters.Add(HeaderParam("api_key", null, true, "api key"));
                        operation.parameters.Add(HeaderParam("app", null, false, "app"));
                        break;
                    //PayOutController
                    case "PayOut_Payouts":
                        operation.parameters.Add(HeaderParam("Authorization", null, true, "Authorization"));
                        operation.parameters.Add(CostumerId);
                        operation.parameters.Add(HeaderParam("countryCode", null, true, "Country code"));
                        break;
                    case "PayOut_List_payouts":
                        operation.parameters.Add(HeaderParam("Authorization", null, true, "Authorization"));
                        operation.parameters.Add(CostumerId);
                        operation.parameters.Add(HeaderParam("countryCode", null, true, "Country Code"));
                        break;
                    case "PayOut_Payout_cancel":
                        operation.parameters.Add(HeaderParam("Authorization", null, true, "Authorization"));
                        operation.parameters.Add(CostumerId);
                        operation.parameters.Add(HeaderParam("TransactionMechanism", null, true, "Transaction Mechanism", "bool"));
                        break;
                    case "PayOut_Payout_update":
                        operation.parameters.Add(HeaderParam("Authorization", null, true, "Authorization"));
                        operation.parameters.Add(CostumerId);
                        operation.parameters.Add(HeaderParam("countryCode", null, true, "Country Code"));
                        operation.parameters.Add(HeaderParam("TransactionMechanism", null, true, "Transaction Mechanism", "bool"));
                        break;
                    case "PayOut_GetClientBalance":
                        operation.parameters.Add(HeaderParam("Authorization", null, true, "Authorization"));
                        operation.parameters.Add(CostumerId);
                        break;
                    default:
                        break;
                }
            }

            readonly Parameter CostumerId  =   new Parameter 
                        { 
                            name = "customer_id",
                            @in = "header",
                            type = "string",
                            description = "Customer id",
                            required = true,
                            exclusiveMaximum = true,                            
                            maximum = 12                            
                        };
            public Parameter HeaderParam(string name, string defaultValue, bool required = true, string description = "", string type = "string")
            {
                return new Parameter
                {
                    name = name,
                    @in = "header",
                    //@ref= defaultValue,
                    @default = defaultValue,
                    type = type,
                    description = description,
                    required = required
                };
            }

            public Parameter BodyParam(string name, string defaultValue, bool required = true, string description = "", string type = "string")
            {
                return new Parameter
                {
                    name = name,
                    @in = "body",
                    //@ref= defaultValue,
                    @default = defaultValue,
                    type = type,
                    description = description,
                    required = required
                };
            }
        }

        public class Consumes : IOperationFilter
        {
            public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
            {
                var attribute = apiDescription.GetControllerAndActionAttributes<SwaggerConsumesAttribute>().SingleOrDefault();
                if (attribute == null)
                {
                    return;
                }

                operation.consumes.Clear();
                operation.consumes = attribute.ContentTypes.ToList();
            }

            [AttributeUsage(AttributeTargets.Method)]
            public class SwaggerConsumesAttribute : Attribute
            {
                public SwaggerConsumesAttribute(params string[] contentTypes)
                {
                    this.ContentTypes = contentTypes;
                }

                public IEnumerable<string> ContentTypes { get; }
            }
        }

        public class PolymorphismSchemaFilter<T> : ISchemaFilter
        {
            private readonly Lazy<HashSet<Type>> derivedTypes = new Lazy<HashSet<Type>>(Init);

            private static HashSet<Type> Init()
            {
                var abstractType = typeof(T);
                var dTypes = abstractType.Assembly
                                         .GetTypes()
                                         .Where(x => abstractType != x && abstractType.IsAssignableFrom(x));

                var result = new HashSet<Type>();

                foreach (var item in dTypes)
                    result.Add(item);

                return result;
            }

            public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
            {
                if (!derivedTypes.Value.Contains(type)) return;

                var clonedSchema = new Schema
                {
                    properties = schema.properties,
                    type = schema.type,
                    required = schema.required
                };

                //schemaRegistry.Definitions[typeof(T).Name]; does not work correctly in SwashBuckle
                var parentSchema = new Schema { @ref = "#/definitions/" + typeof(T).Name };

                schema.allOf = new List<Schema> { parentSchema, clonedSchema };

                //reset properties for they are included in allOf, should be null but code does not handle it
                schema.properties = new Dictionary<string, Schema>();
            }
        }

        public class PolymorphismDocumentFilter<T> : IDocumentFilter
        {
            public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, System.Web.Http.Description.IApiExplorer apiExplorer)
            {
                RegisterSubClasses(schemaRegistry, typeof(T));
            }

            private static void RegisterSubClasses(SchemaRegistry schemaRegistry, Type abstractType)
            {
                const string discriminatorName = "discriminator";

                var parentSchema = schemaRegistry.Definitions[abstractType.Name];

                //set up a discriminator property (it must be required)
                parentSchema.discriminator = discriminatorName;
                parentSchema.required = new List<string> { discriminatorName };

                if (!parentSchema.properties.ContainsKey(discriminatorName))
                    parentSchema.properties.Add(discriminatorName, new Schema { type = "string" });

                //register all subclasses
                var derivedTypes = abstractType.Assembly
                                               .GetTypes()
                                               .Where(x => abstractType != x && abstractType.IsAssignableFrom(x));

                foreach (var item in derivedTypes)
                    schemaRegistry.GetOrRegister(item);
            }
        }
    }
}
