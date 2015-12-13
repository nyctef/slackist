try {
    $ErrorActionPreference = 'Stop'
    push-location $PSScriptRoot/Slackist.Tests
    $env:SLACK_TOKEN = (get-content $PSScriptRoot/slack-token.txt)
    dnx test $args
}
finally {
    pop-location
}
