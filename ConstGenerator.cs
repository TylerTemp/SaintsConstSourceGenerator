using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static SaintsConstSourceGenerator.Utils;

namespace SaintsConstSourceGenerator
{
    [Generator]
    public class ConstGenerator : ISourceGenerator
    {
        private bool _generated;

        public void Initialize(GeneratorInitializationContext context)
        {

        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (_generated)
            {
                return;
            }

            string pathToProject = null;
            // string pathToSave = null;
            // bool found = false;
            foreach (AdditionalText file in context.AdditionalFiles)
            {
                // ReSharper disable once InvertIf
                if (file.Path.EndsWith(".SaintsConstSourceGenerator.additionalfile"))
                {
                    // Utils.DebugToFile($"==={file.GetText()}");
                    // found = true;
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (TextLine textLine in file.GetText().Lines)
                    {
                        // DebugToFile(textLine.ToString());
                        string[] split = textLine.ToString().Split('=');
                        // ReSharper disable once InvertIf
                        if (split.Length == 2)
                        {
                            string key = split[0].Trim();
                            string value = split[1].Trim();
                            // ReSharper disable once ConvertIfStatementToSwitchStatement
                            if (key == "project")
                            {
                                pathToProject = value;
                                // DebugToFile($"pathToSave: {pathToSave}");
                            }
                            else if (key == "debug")
                            {
                                bool setDebug = value != "0";
                                if(Debug != setDebug)
                                {
                                    Debug = setDebug;
                                    DebugToFile($"set debug to {setDebug}");
                                }
                            }

                            // ReSharper disable once InvertIf
                            if (key == "disabled")
                            {
                                // ReSharper disable once InvertIf
                                if (value != "0")
                                {
                                    DebugToFile("plugin disabled");
                                    return;
                                }
                            }
                        }
                    }
                }

                // Utils.DebugToFile($"---file={file.Path}");
                // Utils.DebugToFile($"==={file.GetText()}");

                // if (Path.GetFileName(file.Path) == "myconfig.json")
                // {
                //     var text = file.GetText(context.CancellationToken)?.ToString();
                // }
            }

            if (pathToProject is null)
            {
                DebugToFile("!!!!!!!!!!!!!NOTFOUND");
                return;
            }

            try
            {
                string tagManagerAsset = $"{pathToProject}/ProjectSettings/TagManager.asset";
                TagManagerYaml tagManagerContainer = ParseFile<TagManagerYaml>(tagManagerAsset);
                // DebugToFile($"{tagManagerContainer}");
                // DebugToFile($"{tagManagerContainer.TagManager}");
                // DebugToFile($"{tagManagerContainer.TagManager.tags}");
                // DebugToFile($"{tagManagerContainer.TagManager.tags.Length}");
                TagManagerParser.WriteTagManagerGen(tagManagerContainer.TagManager, context);
                _generated = true;
            }
            catch (Exception e)
            {
                DebugToFile(e.Message);
                DebugToFile(e.StackTrace);
            }
        }

    }
}
