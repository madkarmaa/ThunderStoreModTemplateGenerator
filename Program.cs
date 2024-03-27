using Sharprompt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static Crayon.Output;

namespace ThunderStoreModTemplateGenerator
{

    internal class Program
    {

        private static readonly List<string> quitOpts = new List<string>() { "q", "quit", "exit" };

        static void Main(string[] args)
        {
            string name = Prompt.Input<string>("Name", "MyAwesomeMod", validators: new[] {
                Validators.RegularExpression("^[\\w]+$", "Only letters and underscores are allowed")
            });

            string version = Prompt.Input<string>("Version", "1.0.0", validators: new[] {
                Validators.RegularExpression("^(\\d+)\\.(\\d+)\\.(\\d+)$", "Only 3 numbers separated by dots are allowed")
            });

            string website = Prompt.Input<string>("Website (optional)", validators: new[] {
                Validators.RegularExpression("https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)", "Only website URLs are allowed")
            });

            string shortDesc = Prompt.Input<string>("Short description", "This is my awesome mod", validators: new[] {
                Validators.MaxLength(250, "The description is too long")
            });

            string longDesc = Prompt.Input<string>("Long description (optional)");

            Console.WriteLine($"\nDependencies {Green("(q to quit)")}:\n");

            List<string> dependencies = new List<string>();

            while (true)
            {
                string dependency = Prompt.Input<string>("", validators: new[] {
                    Validators.RegularExpression("^[\\w\\.-]+$", "Invalid dependency name")
                });

                if (quitOpts.Contains(dependency)) break;
                else if (dependency != null) dependencies.Add(dependency);
            }

            if (!Directory.Exists(name)) Directory.CreateDirectory(name);

            File.WriteAllText(name + "/manifest.json", GenerateJsonString(name, version, website, shortDesc, dependencies));
            File.WriteAllText(name + "/README.md", GenerateReadmeString(name, longDesc));

            Console.Write(Bold(Red("\nRemember to add a 256x256 px icon.png image!!! ")));
            Console.WriteLine(Cyan("https://thunderstore.io/c/plasma/create/docs"));
            Console.ReadKey();
        }

        static string GenerateJsonString(string modName, string version, string website, string shortDesc, List<string> dependencies)
        {
            return JsonSerializer.Serialize(new
            {
                name = modName,
                description = shortDesc,
                version_number = version,
                dependencies,
                website_url = website ?? "",
            }, new JsonSerializerOptions { WriteIndented = true });
        }

        static string GenerateReadmeString(string modName, string longDesc)
        {
            return $"# {modName}{(longDesc != null ? $"\n\n{longDesc}" : "")}";
        }

    }

}
