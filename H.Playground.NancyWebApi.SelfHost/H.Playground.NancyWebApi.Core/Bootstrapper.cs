using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using System;
using System.Text;

namespace H.Playground.NancyWebApi.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.AfterRequest.AddItemToEndOfPipeline(context =>
            {
                Console.WriteLine(Print(context));
            });
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("/", @"/Content")
            );
        }

        private static string Print(NancyContext context)
        {
            StringBuilder printer = new StringBuilder();

            printer.AppendLine($"====== START @ {DateTime.Now} ======");
            printer.AppendLine($"Request: {context.Request.Method} {context.Request.Url}");
            printer.AppendLine($"Response: {(int)context.Response.StatusCode}[{context.Response.StatusCode}] {context.Response.ReasonPhrase}");
            printer.AppendLine($"====== END ======");
            printer.AppendLine();

            return printer.ToString();
        }
    }
}
