### Zero to ASP.NET 5

- installed ASP.NET: https://docs.asp.net/en/latest/getting-started/installing-on-windows.html#install-asp-net-5-from-the-command-line
  - `powershell -NoProfile -ExecutionPolicy unrestricted -Command "&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}"`
  - `dnvm upgrade -r coreclr`
- installed node for npm:
  - https://nodejs.org/en/download/
  - `npm install -g npm`
- installed and ran yeoman:
  - `npm install npm install -g yo bower grunt-cli gulp`
  - `npm install -g generator-aspnet`
  - `yo aspnet`
    - Chose a console project
- started building the project:
  - `cd Slackist`
  - `dnu restore`
  - `dnu build`
  - `dnx web` - this runs the webapp and blocks
- poking around:
  - need to figure out what all the bits in project.(lock.)json mean


