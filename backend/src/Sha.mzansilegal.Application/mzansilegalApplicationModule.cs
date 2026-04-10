using System.Reflection;
using System.Threading.Tasks;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Intent.RoslynWeaver.Attributes;
using Sha.mzansilegal.Domain;
using Shesha;
using Shesha.Modules;
using Shesha.Startup;
using Shesha.Web.FormsDesigner;

[assembly: IntentTemplate("Boxfusion.Modules.Application.Services.AppService", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Sha.mzansilegal.Application
{
    [IntentManaged(Mode.Ignore)]
    /// <summary>
    /// mzansilegal Module
    /// </summary>
    [DependsOn(
        typeof(mzansilegalModule),
        typeof(SheshaCoreModule),
        typeof(AbpAspNetCoreModule)
    )]
    public class mzansilegalApplicationModule : SheshaSubModule<mzansilegalModule>
    {
        public override async Task<bool> InitializeConfigurationAsync()
        {
            // Import any configuration embeded as resources in this assembly on application start-up.
            return await ImportConfigurationAsync();
        }

        /// inheritedDoc
        public override void Initialize()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }

        /// inheritedDoc
        public override void PreInitialize()
        {
            base.PreInitialize();

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(SheshaCoreModule).GetAssembly()
                );

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(SheshaApplicationModule).GetAssembly()
                 );

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(SheshaFormsDesignerModule).GetAssembly()
                 );

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(SheshaFrameworkModule).GetAssembly()
                 );

            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(
               typeof(mzansilegalApplicationModule).Assembly,
               moduleName: "mzansilegal",
                useConventionalHttpVerbs: true);

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(mzansilegalModule).GetAssembly()
                 );

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(mzansilegalApplicationModule).GetAssembly()
                 );
        }
    }
}
