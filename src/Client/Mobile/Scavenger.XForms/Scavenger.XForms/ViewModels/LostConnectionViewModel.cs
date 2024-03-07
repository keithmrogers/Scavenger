using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.XForms.Services;
using Xamarin.Forms;

namespace Scavenger.XForms.ViewModels
{
    public class LostConnectionViewModel : BaseNavigationViewModel
    {
        private readonly ICloseApplicationService _closeApplicationService;
        public LostConnectionViewModel()
        {
            _closeApplicationService = DependencyService.Get<ICloseApplicationService>();
        }

        Command _closeCommand;
        public Command CloseCommand => _closeCommand ??
                                               (_closeCommand = new Command(ExecuteCloseCommand));

        private void ExecuteCloseCommand()
        {
            _closeApplicationService.Close();
        }
    }
}
