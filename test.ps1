try {
    $ErrorActionPreference = 'Stop'
    push-location $PSScriptRoot/Slackist.Tests
    $env:SLACK_TOKEN = (get-content $PSScriptRoot/slack-token.txt)
    $env:TODOIST_TOKEN = (get-content $PSScriptRoot/todoist-token.txt)
    dnx test $args
}
finally {
    pop-location
}
