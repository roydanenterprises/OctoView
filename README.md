# OctoView

OctoView is a general copy of [kingofzeal/GithubDashboard](https://github.com/kingofzeal/GithubDashboard), with the intention of creating a more robust front-end than was originally planned. 

The intention of this project is to create a website that is an alternative view of certain Github information. Specifically, instead of being repository-focused, this is branch/feature-focused.

## General Vision
1. Users log into a website
2. Users will see a list of branches attached to every repository as the primary grouping.
  a. Under each branch will be easy repository that branch is found in.
  b. If the branch has a Pull Request associated with it, it will show information about the PR, including reviews and merge status
  
This allows for an easier and automated view of projects that span multiple repositories.

## Additional features
1. Users can customize which repositories are examined
2. Users can manually associate/group branches (eg, `feature/featureAbcdefg` and `feature/featureAbcdef` are the same 'project' and thus get shown together)
3. Most information shown links to the Github source for that information (eg, PRs, reviews, branches, repos)

