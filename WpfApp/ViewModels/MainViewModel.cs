using Generators;
using WpfApp.Models;

namespace WpfApp.ViewModels
{

    /// <summary>
    /// Source generator Crashed!
    ///
    /// Try build App!
    ///
    /// Application build ok editor crashed!
    /// After restart IDE project work OK
    ///
    ///
    ///  Random down on VS2022, on VS2019 work all time perfected!
    ///
    /// Working!
    /// And OK!
    /// 
    /// </summary>
    public partial class MainViewModel : BindableBase
    {
        [AutoNotify(generateOnChanged: true)]
        private string _textInput = "Try change text";

        [AutoNotify]
        private string _textOutput;

        partial void OnTextInputChanged()
        {
            TextOutput = TextInput;
        }
    }
}
