try {
    $ErrorActionPreference = 'Stop'
    $slackToken = (get-content $PSScriptRoot/slack-token.txt)
    $todoistToken = (get-content $PSScriptRoot/todoist-token.txt)
    push-location $PSScriptRoot/Slackist
    dnu restore
    dnx Slackist $slackToken $todoistToken
}
finally {
    pop-location
}
