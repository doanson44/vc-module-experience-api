using AutoMapper;
using GraphQL.Server;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.ExperienceApiModule.XProfile.Mapping;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Mapping;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Mapping;

namespace VirtoCommerce.ExperienceApiModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection services)
        {
            //services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
            //serviceCollection.AddSingleton(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>));

            //Discover the assembly and  register all mapping profiles through reflection
            services.AddAutoMapper(typeof(XDigitalCatalogAnchor));
            //services.AddAutoMapper(typeof(XProfileAnchor));
            services.AddAutoMapper(typeof(XPurchaseAnchor));

            //Register .NET GraphQL server
            var graphQlBuilder = services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
            }).AddNewtonsoftJson(deserializerSettings => { }, serializerSettings => { })
            .AddUserContextBuilder(context => new GraphQLUserContext { User = context.User })
            .AddRelayGraphTypes()
            .AddDataLoader();

            //Register custom GraphQL dependencies
            services.AddPermissionAuthorization();

            services.AddSingleton<ISchema, SchemaFactory>();

            // Register core schemas
            services.AddSchemaType<MoneyType>();

            //Register all purchase dependencies
            services.AddXCatalog(graphQlBuilder);
            services.AddXProfile(graphQlBuilder);
            services.AddXPurchase(graphQlBuilder);
            //TODO: need to fix extension, it's register only types from the last schema
            //services.AddGraphShemaBuilders(typeof(Anchor));

            //Discover the assembly and  register all mapping profiles through reflection
            //TODO: Not work for profiles defined in the different projects
            //services.AddAutoMapper(typeof(Module));

            //TODO: Need to find proper way to register mapping profiles from the different projects
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
                mc.AddProfile(new ProductMappingProfile());
                mc.AddProfile(new CartMappingProfile());
                mc.AddProfile(new ProfileMappingProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            // add http for Schema at default url /graphql
            appBuilder.UseGraphQL<ISchema>();

            // use graphql-playground at default url /ui/playground
            appBuilder.UseGraphQLPlayground();
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }
    }
}
