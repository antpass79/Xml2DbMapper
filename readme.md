# Xml2DbMapper

The library takes xml files (and txt files) as input and maps on a database all values as intermediate step.

It finally produces xml files as output.

## How to connect to the Database

Follow the below steps:

- Set *Xml2DbMapper.Core* as the default project in the *Package Manager Console*

- Install the right package from *Package Manager Console*:

        Install-Package Microsoft.EntityFrameworkCore.Tools
        Install-Package Microsoft.EntityFrameworkCore.SqlServer

- Generate the models

        Scaffold-DbContext "Server=<Database Instance>;Database=<Database Name>;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir <Output Folder>

## How to set the CI/CD

Follow the below steps:

- Create a repository on github with only the readme.md file

- Create the *feature* branch

- From cmd prompt, type:

        git clone <Repository Url>

- Switch to the feature branch in order to commit into this branch:

        git checkout feature

## References

### Database - Database First

 - <https://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx>

### Azure DevOps - CI/CD

- <https://cloudblogs.microsoft.com/industry-blog/en-gb/technetuk/2019/06/18/perfecting-continuous-delivery-of-nuget-packages-for-azure-artifacts/>
- <http://thecollaborationcorner.com/2019/02/28/azure-devops-with-spfx-gitflow-gitversion/>
- <https://stackoverflow.com/questions/55768926/logon-failed-use-ctrlc-to-cancel-basic-credential-prompt-to-git-in-azure-devop>

### Azure DevOps - Github Sync

- <https://ngohungphuc.wordpress.com/2018/12/26/sync-between-github-and-vsts-azure-devops-repo/>
- <https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page>

### Git - Commands

- <https://devconnected.com/how-to-switch-branch-on-git/>

### Git - Branches

- <https://amerlin.keantex.com/git-merge-vs-rebase/> (italian)