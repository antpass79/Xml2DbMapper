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

## CI/CD

### AzureDevOps - Branches

- <https://cloudblogs.microsoft.com/industry-blog/en-gb/technetuk/2019/06/18/perfecting-continuous-delivery-of-nuget-packages-for-azure-artifacts/>

### Git - Commands

- <https://devconnected.com/how-to-switch-branch-on-git/>

### Git - Branches

- <https://amerlin.keantex.com/git-merge-vs-rebase/> (italian)