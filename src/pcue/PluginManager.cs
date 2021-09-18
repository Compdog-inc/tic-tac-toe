using Compdog.TicTacToe.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Compdog.TicTacToe.Internal.PCUE
{
    internal class PluginManager
    {
        private TicTacToe.PCUE pcue;
        private IPlugin[] loadedPlugins;

        internal PluginManager(TicTacToe.PCUE pcue)
        {
            this.pcue = pcue;
            this.loadedPlugins = new IPlugin[0];
        }

        internal bool Load(bool allowDuplicates)
        {
            bool verbose = pcue.Arguments.HasFlag("v");
            Console.Write("[PLUGMAN] searching for plugins... ");
            if (!Directory.Exists("plugins/"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("not found");
                Console.ForegroundColor = ConsoleColor.Gray;
                return false;
            }

            string[] files = Directory.GetFiles("plugins/", "*.dll");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"found {files.Length}");
            Console.ForegroundColor = ConsoleColor.Gray;

            List<IPlugin> plugins = new List<IPlugin>();

            for (int i = 0; i < files.Length; ++i)
            {
                if (verbose)
                {
                    Console.Write($"[PLUGMAN] loading ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(Path.GetFileName(files[i]));
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("... ");
                }

                try
                {
                    Assembly asm = Assembly.Load(File.ReadAllBytes(Path.GetFullPath(files[i])));

                    if(verbose)
                        Console.Write("done ");

                    AssemblyName name = asm.GetName();

                    if (verbose)
                    {
                        Console.Write("i: ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(name.Name);
                        Console.Write(" v");
                        Console.Write(name.Version.ToString());
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(" ");
                    }

                    Type type = (
                        from t in asm.GetTypes()
                        where t.IsClass && t.GetInterface(typeof(IPlugin).FullName) != null
                        select t
                        ).SingleOrDefault();

                    if (verbose)
                    {
                        Console.Write("t: ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(type == null ? "NULL" : type.ToString());
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("... ");
                    }

                    if (type == null)
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("FAIL");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        continue;
                    }

                    if (plugins.Any(b => type.FullName.Equals(b.GetType().FullName) && asm.GetName().Version.Equals(b.GetType().Assembly.GetName().Version)))
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("DUPLICATE");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }

                        if (allowDuplicates)
                        {
                            if(verbose)
                                Console.Write(" (allowed)... ");
                        }
                        else
                        {
                            if(verbose)
                                Console.WriteLine();
                            continue;
                        }
                    }

                    object oplugin = asm.CreateInstance(type.FullName);
                    if (oplugin == null)
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("FAIL");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        continue;
                    }

                    IPlugin plugin = (IPlugin)oplugin;

                    if (!plugin.Load(pcue))
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write("load returned false");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write("... ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("FAIL");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        continue;
                    }

                    plugins.Add(plugin);

                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("OK");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                catch (Exception e)
                {
                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR");
                        Console.WriteLine(e.ToString());
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }

            Console.WriteLine($"[PLUGMAN] loaded {plugins.Count} plugins");
            loadedPlugins = plugins.ToArray();
            return true;
        }
        internal bool Unload()
        {
            if (loadedPlugins.Length <= 0)
            {
                Console.WriteLine("[PLUGMAN] no plugins to unload.");
                return false;
            }

            bool ok = true;

            foreach (IPlugin plugin in loadedPlugins)
            {
                if (!plugin.Unload())
                {
                    ok = false;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[PLUGMAN] {plugin.GetType().Assembly.GetName().Name} returned false while unloading");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            loadedPlugins = new IPlugin[0];
            Console.WriteLine("[PLUGMAN] unloaded all plugins.");
            return ok;
        }
        internal bool TrySearch(string query, out IPlugin[] plugins)
        {
            List<Version> parsedVersions = new List<Version>();
            List<string> split = query.Split(' ').ToList();
            split.ForEach(s =>
            {
                if (s.StartsWith("V"))
                {
                    if (Version.TryParse(s.Substring(1), out Version v))
                    {
                        parsedVersions.Add(v);
                    }
                }
            });

            bool searchQuery(IPlugin p)
            {
                string typeName = p.GetType().FullName.ToUpperInvariant();
                AssemblyName asmName = p.GetType().Assembly.GetName();
                string name = asmName.Name.ToUpperInvariant();

                if (!split.Any(s => typeName.Contains(s) || name.Contains(s)))
                {
                    return false;
                }

                if (parsedVersions.Count == 0) return true;

                return parsedVersions.Any(v => v.Equals(asmName.Version));
            }

            IPlugin[] validPlugins =
                (from p in loadedPlugins
                 where searchQuery(p)
                 select p).ToArray();

            if (validPlugins.Length <= 0)
            {
                plugins = null;
                return false;
            }

            plugins = validPlugins;
            return true;
        }
        internal IPlugin[] Search(string query)
        {
            if (TrySearch(query, out IPlugin[] plugins))
                return plugins;
            else
                throw new KeyNotFoundException("No plugins found");
        }
        internal IPlugin[] List()
        {
            return loadedPlugins.ToList().AsReadOnly().ToArray();
        }
    }
}
