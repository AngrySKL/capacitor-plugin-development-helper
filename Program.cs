
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

string pluginProjectName = args[0];
string pluginName = args[1];

Console.WriteLine("======== 找到插件{0}项目的package.json并增加小版本号 ========", pluginProjectName);
string pluginPath = @"/Users/wangyilei/git/ng-images-mobile-plugin/" + pluginProjectName + "/";
var pluginJsonFile = pluginPath + "package.json";
JObject pluginJson = JObject.Parse(File.ReadAllText(pluginJsonFile));
JToken pluginVersion = pluginJson["version"] as JToken;
string[] versionArray = ((string)pluginVersion).Split(".");
versionArray[2] = (Convert.ToInt32(versionArray[2]) + 1).ToString();
string newPluginVersion = string.Join(".", versionArray);
pluginJson["version"] = newPluginVersion;
File.WriteAllText(pluginJsonFile, JsonConvert.SerializeObject(pluginJson, Formatting.Indented));

string buildNewPackageCommand = "cd " + pluginPath + " && npm run build";
buildNewPackageCommand.Bash();

Console.WriteLine();
Console.WriteLine("======== 拷贝新包 ========");
string newPackageName = pluginName + "-" + newPluginVersion.ToString() + ".tgz";
string copyNewPackageCommand = "cp "
+ @"/Users/wangyilei/git/ng-images-mobile-plugin/" + pluginProjectName + "/dest/" + newPackageName
+ @" /Users/wangyilei/git/ng-images-mobile/src/local-packages/teamplay-plugins/" + newPackageName;
copyNewPackageCommand.Bash();

Console.WriteLine();
Console.WriteLine("======== 移除旧包并更新安卓项目引用 ========");
string androidProjectPath = @"/Users/wangyilei/git/ng-images-mobile/";
var projectJsonFile = androidProjectPath + "package.json";
JObject projectJson = JObject.Parse(File.ReadAllText(projectJsonFile));
string oldPackageFilePath = ((string)projectJson["dependencies"][pluginName]).Replace("file:", androidProjectPath);
string removeOldPackageCommand = "cd " + androidProjectPath + " && rm " + oldPackageFilePath;
removeOldPackageCommand.Bash();
projectJson["dependencies"][pluginName] = "file:src/local-packages/teamplay-plugins/" + pluginName + "-" + newPluginVersion + ".tgz";
File.WriteAllText(projectJsonFile, JsonConvert.SerializeObject(projectJson, Formatting.Indented));
string updateDependencyCommand = "cd " + androidProjectPath + " && npm i --force && ionic cap sync android";
updateDependencyCommand.Bash();

Console.WriteLine();
Console.WriteLine("======== done ========");