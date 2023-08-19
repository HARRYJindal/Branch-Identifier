using AutoMapper;
using BranchCleaner.Mapper;
using BranchCleaner.Models;
using BranchCleanerDll.Interfaces;
using BranchCleanerDll.Model;
using BranchCleanerDll.Repository;

namespace BranchCleaner
{
    public class Startup
    {
        private MapperConfiguration _mapperConfiguration;

        public Startup()
        {
            _mapperConfiguration = new MapperConfiguration(
                ctg =>
                {
                    //ctg.AddProfile(new BranchDays_BranchDate());
                    ctg.CreateMap<BranchDate,BranchDays>();
                }
            );
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IBranchCleaner, BranchCleanerRepository>();
            services.AddControllers();
            services.AddSingleton<IMapper>(sp => _mapperConfiguration.CreateMapper());

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
        }

        /// <summary>
        /// it is used to configure the http requests and response by adding the middleware
        /// </summary>
        /// <param name="applicationBuilder">it is used to help adding middlewares</param>
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            // to redirect the http requests to https requests
            applicationBuilder.UseHttpsRedirection();
            // to enable cross origin resource sharing with my policy
            applicationBuilder.UseCors("MyPolicy");
            applicationBuilder.UseRouting();
            applicationBuilder.UseRouting();


            // to point the routes to spefic action method 
            applicationBuilder.UseEndpoints((endpoints) =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=BranchCleaner}/{action=GetBranches}/{id?}");
            });

        }
    }
}
