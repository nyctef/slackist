try {
    $slackToken = (get-content $PSScriptRoot/slack-token.txt)
    $todoistToken = (get-content $PSScriptRoot/todoist-token.txt)
    push-location $PSScriptRoot/Slackist
    dnx Slackist $slackToken $todoistToken
}
finally {
    pop-location
}
