A local ASP .NET Core web service to provide git status caching for use with PoshGit.

Instructions:
1. ```git clone https://github.com/ali-hk/posh-git.git```
2. ```git checkout status-cache-service```
    * Some modifications were made to posh-git to enable this status cache
3. ```git clone https://github.com/ali-hk/GitStatusCache.git```
4. ```git checkout wip-watcher```
5. Add the following to your PowerShell profile (```%USERPROFILE%\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1```)
    1. ```Import-Module '<path-to-posh-git-clone>\posh-git\src\posh-git.psd1'```
    2. ```$global:GitPromptSettings.EnableFileStatusFromCache=$true```
    3. ```$global:GitPromptSettings.StatusCacheHostName='localhost:5000'```
6. In a new command window: Run ```<path-to-git-status-cache-clone>\src\GitStatusCacheService\run.cmd```
    * Keep this window open when using the cache
    * Alternatively, you can setup a scheduled task to run on startup "Run whether user is logged on or not" and "Do not store password". There won't be a command window visible that way.

Enjoy faster prompts with posh-git!
