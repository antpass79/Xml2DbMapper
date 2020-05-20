# Xml2DbMapper

The repository contains 2 folders:

- **Library**: a library that takes xml files (and txt files) as input and maps on a database all values as intermediate step. It finally produces xml files as out
- **Host**: a console application that uses the above library to process files

The is a *solution* for each one.

## How to connect to the Database

The connection to the database in order to generate all models (database first approach) is related to the projects inside the *Library* folder.

Follow the below steps:

- Open the *Xml2DbMapper.Library.sln* inside *Visual Studio 2019*

- Set *Xml2DbMapper.Core* as the default project in the *Package Manager Console*

- Install the right package from *Package Manager Console*:

        Install-Package Microsoft.EntityFrameworkCore.Tools
        Install-Package Microsoft.EntityFrameworkCore.SqlServer

- Generate the models

        Scaffold-DbContext "Server=<Database Instance>;Database=<Database Name>;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir <Output Folder>

## How the CI/CD works

The repository is hosted on *Github*. There are 2 branches:

- **master**: this branch must always be functional
- **feature**: it's the branch from which other branches start, based on activities to develop
- other main branches can be created, for example: *bug-fix* or others

Steps in charge of the developer:

- the developer works on personal branches created from *feature* branch, based on a specific activity
- commits are done on the specific branch
- once an activity is terminated and all commits are done, the developer makes a *pull request* towards the *feature* branch
- if the branch doesn't need anymore, the developer can delete the branch

Steps to produce alpha artifacts:

- once the pull request is done by a developer, the build pipeline runs on the *feature* branch
- if the build fails, the developer must commit changes in order to solve the problem
- if the build works well, the artifacts are put in the feed. These artifacts are tagged as alpha

This artifacts can be used by the Xml2DbMapper.Host, using *NuGet Package Manager*, in order to test the stability.

Steps to produce stable artifacts:

- once the build on *feature* branch is gone well, a pull request on the *master* branch can be done by reviewers that check the code
- once the build finishes, the artifacts are put in the feed. These artifacts are not tagged. They can be promoted as *Release* in order to avoid delete by *Retention Policies*  

## Some git commands

To download a repository from github, type from command prompt:

        git clone <Repository Url>

To use a branch, type from command prompt:

        git checkout <Branch Name>

To list all branches, type from command prompt:

        git branch -a

## References

### Database - Database First

 - <https://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx>

### Azure DevOps - CI/CD

- <https://cloudblogs.microsoft.com/industry-blog/en-gb/technetuk/2019/06/18/perfecting-continuous-delivery-of-nuget-packages-for-azure-artifacts/>
- <http://thecollaborationcorner.com/2019/02/28/azure-devops-with-spfx-gitflow-gitversion/>

### Azure DevOps - Github Sync

- <https://ngohungphuc.wordpress.com/2018/12/26/sync-between-github-and-vsts-azure-devops-repo/>
- <https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page>
- <https://stackoverflow.com/questions/55768926/logon-failed-use-ctrlc-to-cancel-basic-credential-prompt-to-git-in-azure-devop>

### Git - Commands

- <https://devconnected.com/how-to-switch-branch-on-git/>

### Git - Branches

- <https://amerlin.keantex.com/git-merge-vs-rebase/> (italian)