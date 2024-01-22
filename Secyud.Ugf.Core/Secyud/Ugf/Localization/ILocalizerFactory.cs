#region

using System.Globalization;

#endregion

namespace Secyud.Ugf.Localization
{
    /// <summary>
    /// 用于管理语言功能以及所有的翻译器。当翻译器被注册时，
    /// 应当同时使用此接口进行管理，以便对所有翻译器调用相同操作。
    /// </summary>
    public interface ILocalizerFactory 
    {
        void ChangeCulture(CultureInfo cultureInfo);
        void RegisterLocalizer(ILocalizer localizer);
    }
}