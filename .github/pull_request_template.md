<!---
	Leaving any section of the following PR blank can result in requested changes for the PR, and will most-likely not be considered until all the sections are _meaningfully_ filled out. Any text, which is not commented should be included in the PR. Please, do not delete it. 
--->

# PR Summary 

This PR closes github issue <!--- Place #NUM here, e.g. #102 --->. <!--- Please any further summary here. Generics like what this issues solved in 1 setnence or less, and the highlights from the associated issue that are helpful are always good to include here - also, feel free to mention anyone that may need to look at this PR. --->

## IDE Improvements 

```
Replace this section with a copy-pasta using https://github.com/youar/uberarfoundation/blob/staging/docs/InspectorResultsRider.png, then ctrl+A, then ctrl+C, and :spaghetti: Should end up look similar to the following section: 

Found issues
IssueGroupingObjectModel(groupName=Severity, text=WARNING, icon=ImageSourceIconModel (iconPackStringId = "SolutionAnalysis"iconNameStringId = "SolutionAnalysisWarning"))
IssueGroupingObjectModel(groupName=Severity, text=ERROR, icon=ImageSourceIconModel (iconPackStringId = "SolutionAnalysis"iconNameStringId = "SolutionAnalysisError"))
IssueGroupingObjectModel(groupName=Severity, text=SUGGESTION, icon=ImageSourceIconModel (iconPackStringId = "SolutionAnalysis"iconNameStringId = "SolutionAnalysisSuggestion"))
IssueGroupingObjectModel(groupName=Severity, text=HINT, icon=ImageSourceIconModel (iconPackStringId = "SolutionAnalysis"iconNameStringId = "SolutionAnalysisHint"))

Delete this auto-generated text once complete. 
```

And the image showing this improvements, in particular those for the <!--- Particular Namespace ---> namespace, 

<!--- 
Please add an image of the error list here, similar to  
![image](https://user-images.githubusercontent.com/13577108/86987069-0bec5600-c14a-11ea-8394-99b931d4c353.png)
---->

Ultimately, we have moved from <!--- Starting Error Num ---> errors to <!--- Ending Error Num ---> issue with internal changes in this branch.

## QA Summary 

## :ledger: Notes

No additional notes were given.

## Release Checklist

## Testing
- [ ] Build app for Android
- [ ] Build app for IOS
- [ ] Make sure `README.md` instructions to build server and client work

### Prepare Project

- Visit the project's releases page (e.g. `open https://github.com/youar/upm-xrremote/releases`)
	- [ ] Create a new release object for your git tag (i.e. `v$VERSION`).
	- [ ] Add `CHANGELOG`
- Update packages
	- [ ] `upm-xrremote.apk`

### Update Documentation
- [ ] Update `README.md` as necessary
	- [ ] Update requirements
	- [ ] Document new features

#### Build Status

- [x] Build Passes ✅ 
- [x] Build Fails :x: 
