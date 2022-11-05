# Development Practices

This document should be read by all contributors of this repository. It will cover concepts of [GitHub](#github) usage, best [Git Practices](#git), and how to [commit](#commits) properly. 

_For information on how to use Rider and ReSharper, take a look at [this document](ReSharperNotes.md)_

**TL;DR**

- We use git flow;  
- Branch, only if there is a GitHub issue for it.
- Branch, name
```<feature,enhancement,hotfix>_<featureNameFromGithubCamelCaseIssue(#issueNumberWithoutHashtag)>[_<coderFirstNameAndLastInitial>]```
- Merging, only done via PR.
- Merging, only done by project manager or feature manager.
- Commit messages, follow `scope/tag:` syntax
- Creating a PR requires completing the template and fulfilling the following, 
	- Your implementation has corresponding Tests
	- Your implementation has shown linting improvements
	- Your application builds
	- Your code is _well documented_ 
- Once a PR is ready, please make sure you have let the CI complete, and that all check are successful. 
- Test a local version of your application - make sure the basic Android (iOS) build builds, and passes basic functionality (note that this doesn't have to be done for _every_ push to a PR **just the last one**)
- request review from the appropriate person (they may ask for changes)

> If you haven't, please add the commit.template file at `./.git_commit_template` to your git settings via 
> ```git config commit.template ./.git_commit_template```
> _If you are on SourceTree, this may not work, and you should either use command line for commiting, or switch to CLI_

## GitHub

In tandem to our use of git, we employ extensive use of GitHub as a management tool to discuss features, milestones, projects, bugs, release and more. This section is designed to cover how GitHub should be used in the context of our development. It also highlights why it's an important step before dealing with Git or Code directly. 

### Planning

Planning a project comprises laying out the tasks to be done. Project planning is done via GitHub Projects, which utilizes the GitHub issues/features and pull request pages to related issues/features to track who is working on what, when it is integrated and what issues/features are grouped together. 

Successful planning results in a list of GitHub issues/features which have tags and projects associated with them, along with _assignees so that responsibility of the task lands on the programmer._ Along with the tags, issues, and features we scope out for the release, or updates of the software, we use GitHub Pull Requests for operations like squashing, merging, and deleting branches.

### Pull Requests

Pull Requests (PRs) are essential to every feature or issue that wants integration into the main branch of software and does indeed mark "completion" of a task. Pull requests make up a majority of the development flow and it is established as the following.

1. The programmer selects an assigned task for the sprint.
2. The programmer consults with the feature or project manager about branching. 
3. Develops
4. Once the programmer feels like their GitHub issue or feature has been resolved they should make a pull request from their branch into the target base branch. _Remember this can always be derived from the branch name if you forget - you can always ask the feature or project manager if you forget_. 
5. The feature or project manager performs a "Squash and Merge"
6. The feature or project manager determines if the branch should be deleted or not - it usually should be. 
7. Go to (1)

**Notes on Pull Requests**

Titles of PRs can be a bit tricky. There are no rules, but try to stick with the assumption that the title should _probably_ have a verb in it, and try to avoid generics like "improvement", "enhancement" without direct relationship to a GitHub issue. For instance the title "GitHub Issue Hotfix #394" is an appropriate title, but "Hotfix for Infinite Download Loop" should be avoided; "Hotfix for Infinite Download Loop" is for sure a situation that might happen again, nor does the title really highlight what the Hotfix fixed. 

The other main purpose of the PR is to allow the feature or project manager the ability to Squash And Merge commits. What this really does is rebase your commit history onto itself, interactively, allowing you to have only 1 commit which reflects the feature/issue changes. At this point you can edit a N commit push into 1 commit, for which other programmers won't have to sift through information on the minor bug fixes or progress updates: We only care about the result for keeping track of software progress. For example using "Squash and Merge", we can edit this commit message merge mess, 

```
commit E
Author: gblikas <gblikas@gmail.com>
	fix: working implementation

commit D
Author: gblikas <gblikas@gmail.com>
	wip: house on fire

commit C
Author: gblikas <gblikas@gmail.com>
	fix: WingardiumLeviosa.wand didn't work

commit B
Author: gblikas <gblikas@gmail.com>
	add/function: WingardiumLeviosa.wand

commit A
Author: gblikas <gblikas@gmail.com>
	feat: initial WingardiumLeviosa implementation
```
into something more concise and useful on the main branch, like

```
commit N	
Author: gblikas <gblikas@gmail.com>
	feat/enhancement: WingardiumLeviosa for github issue #394
```

Notice that the person using the code base doesn't really need to know that it was broken at one point, nor that gblikas' house caught fire. 

_Squash and Merge with a branch delete should always be preferred_. This keeps our repository digestible, and easily to analyze from the outside, allow use to on-board or switch tasks easier. 

#### Accepting Pull Requests

Accepting pull requests has to do with both code quality assurance and functional quality assurance. With a smaller team, unfortunately, first levels of triage require the coder to make sure that any modifications to the code base also pass _functional_ quality assurance. In the future, I hope our team expands, so that the programers are not the ones doing functional testing. The following is designed to mitigate issues with newer release, but unfortunately requires a little more work for each PR.

We use the following levels of QA severity, in numerical order, to stress imporantance in the QA process: 

1. EFFECTIVE - the application accomplishes the _complete_ goal.
2. FUNCTIONAL - the code base accomplishes basic operations necessary for the target goal.
3. FOUNDATIONAL - the code base needs this for "high-level" development of functions. 
4. ITERABLE - we can properly iterate on software

**_As of `preview-v4.0.0`, the YOUAR Cloud SDK is no longer accepting PRs and code accomplishes the following_**:
- (ITERABLE) Code is documented and written
- (FOUNDATIONAL) Code is Linted
	- Please see this link on [Rider and ReSharper] and how we use it for PRs. 
	- Please, make sure to attach a build summary output of `CodeInspection` from your project.  
- (FUNCTIONAL) Code has corresponding unit tests, when tests are applicable
- (EFFECTIVE) QA report passes for API(s)
	- The QA sheet must be updated if the API modifications include more features than were previously there
	- It's a reasonable assumption that you can cut corners when making smaller modifications. **HOWEVER**, _the responsibility of app failure will fall on you, if you do so_; if there is unit test/functional test in the QA sheet and said test fails with your build, it's assumed you didn't actual finish the GitHub Issue. 

## Git

Integral to our development and success as a team is consistent use of Git at several different levels of development; git as a tool for keeping track of code; as a tool for documenting high-level, human readable changes in the code base; GitHub as an effective communication tool for other developers and consumers of the code-base.

Before we dive into these sections, lets take a look at the diagram which we utilize for our git [branching structure](https://nvie.com/posts/a-successful-git-branching-model/):

![You can ignore the "Release" branch](https://nvie.com/img/git-model@2x.png) 

Here, you can see that master is _as close to possible_ to just the "tags", aka versions, of the code base as possible. In practice `master` will have a few more commits, but shouldn't have many more. `master` branch is used as a milestone marker of the largest progress made. That is, if the `develop` or `staging` branch were to be deleted tomorrow, we could go back to master and not lose signification progress. 

### Git and Code

Using git as a way to to keep track of code as a temporary "ctrl+z" is just fine, but needs to follow some rules so that commit branches aren't riddled with "wip: bathroom", and the like. In general, using git as a tool for keeping track of coding changes is really a discussion about branching, merging and how commit messages are formated. 

#### Branching

Branching should follow these rules for branching and naming the branch, 

- Every branch of development is broken up into only three categories, `feature`, `enhancement`, `hotfix`
- Every branch of development should have a corresponding GitHub issue, or feature request to relate the purpose of the branch to a target goal
- Every branch of development should have the "base syntax" 
```<feature,enhancement,hotfix>_<featureNameFromGithubCamelCaseIssue(#issueNumberWithoutHashtag)>```
- Branches which are offshoots from development branches should follow the "inherited syntax", 
```<feature,enhancement,hotfix>_<featureNameFromGithubCamelCaseIssue(#issueNumberWithoutHashtag)>_<coderFirstNameAndLastInitial>```
- Branches besides `master` and `staging` have a finite life time (they should be deleted)

These are two examples of properly named feature branch, with developers contributing to them

```
feature_arcoaching
feature_arcoaching_georgeb
feature_arcoaching_vacharal

hotfix_infiniteDownloadLoopIssue394
hotfix_infiniteDownloadLoopIssue394_jond
```

This implies some interesting things from the project management standpoint: 
1. If you are creating a branch and don't have a GitHub issue, the project manager dropped the ball and you are (should be) blocked; more planning should have happened.  
2. You may not be going toward a concrete feature, bugfix, or enhancement, which may be hard to communicate with the others who are using your software/code. 
3. You might not be the project or feature manager and are taking on responsibility that isn't yours. 
4. You need something in the software that isn't there and you should make a GitHub issue so that (1) is valid. 

#### Merging

In the previous section on [Git and Code](#git-and-code), we covered naming conventions and how to branch. But, what about how to merge?

The 2 most important rule is that, _merging should not be done by any other coder for `staging` and `master` other than the project maintainer, and that tagging should not occur on any other branch except master. Further the merges into `staging` and `master` should only ever be done via a PR_. Outside of that, the feature manager (to be assigned at GitHub feature or issue postmortem), is responsible for the branch `<feature,enhancement,fix>_<featureNameFromGithubCamelCaseIssue(#issueNumberWithoutHashtag)>`, and adopting a similar flow to the one outlined by the image above, 

```
feature_A        --- a --- b --- c --- d --- e ---
                      \              /
feature_A_janed        b'--- q --- f
```

#### Commits

Although it's not creating a branch object, or merging histories, equally important is a commit message, how it's formatted and what it should communicate. There is further information on how to do this [here](.github/actions/changelog/README.md). Here is a quick example using the format outline by the referenced document (`scope/tag:` style): 

```
fix/security: loophole with authentication method, so that users didn't actually need an authentication key to be validated.

add/function: getNonce() for added security in the handshake that occurs with our server. 

docs/conceptual: fixed spelling errors in documentation files for tutorials on Uber.
```

Additionally, using commit messages as a method for changelogs is quite popular and we adopt it with the hope that it should further solidify a high-level connection between what the goal we set out to accomplish and what was ultimately merged into the software. 

