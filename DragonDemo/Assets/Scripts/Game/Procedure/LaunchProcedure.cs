using Config;
using System.Threading.Tasks;
using TGame.Procedure;

public class LaunchProcedure : BaseProcedure
{
    public override async Task OnEnterProcedure(object value)
    {
       UnityLog.Info("enter Launch procedure");
#if !UNITY_EDITOR
            UnityEngine.PlayerPrefs.DeleteKey(UnityEngine.AddressableAssets.Addressables.kAddressablesRuntimeDataPath);
#endif
        //await Addressables.InitializeAsync();
        await LoadConfigs();
        await ChangeProcedure<InitProcedure>();
        await Task.Yield(); 
    }

    private async Task LoadConfigs()
    {
        UnityLog.Info("===>º”‘ÿ≈‰÷√");
        ConfigManager.LoadAllConfigsByAddressable("Assets/BundleAssets/Config");

        GlobalConfig.InitGlobalConfig();
        BuffConfig.ParseConfig();
        SkillConfig.ParseConfig();
        BulletConfig.ParseConfig();
        SpellFieldConfig.ParseConfig();
        I18NConfig.ParseConfig();
     

       // await Task.Yield();
        UnityLog.Info("<===≈‰÷√º”‘ÿÕÍ±œ");
    }
}
