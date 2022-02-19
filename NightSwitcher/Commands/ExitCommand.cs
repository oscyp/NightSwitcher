using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NightSwitcher.Commands
{
    public class ExitCommand : CommandBase<ExitCommand>
    {
        public override void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
