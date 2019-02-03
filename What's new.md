# BiliCommenter What's new

## 0.2.x - currently mainly supporting version

### 0.2.2.0

New features:

```
Welcome window.
```

Fixed the task loading bug in 0.2.1.0.

### 0.2.1.0

Changes:

```
1.We're goting to stop submitting the file changes to this file.
2.Some changes to README.md.
```

New features:

```
	#1 Console (cross-platform, .NET Core) version of this application that can read the tasks.json file and execute the tasks, although it's hard to use.
	#2 @ManiaciaChao designed a logo for BC/WPF project.
```

Fixed some bugs in 0.2.0.0

### 0.2.0.0 [Buggy]

New features:
```
	#1 What's new document.
	#2 Settings flyout.
	#3 Task storage.
	#4 Behaviour of CommentTask and Noticer were changed.
	#5 Do NOT load emoji pictures while debugging.
```
Changes to be committed:
```
	new file:   What's new.md
	modified:   BiliCommenter/App.config
	modified:   BiliCommenter/MainWindow.xaml
	modified:   BiliCommenter/MainWindow.xaml.cs
	modified:   BiliCommenter/Properties/AssemblyInfo.cs
	modified:   BiliCommenter/Properties/Settings.Designer.cs
	modified:   BiliCommenter/Properties/Settings.settings
	modified:   BiliCore/BiliCore.csproj
	modified:   BiliCore/Core/CommentTask.cs
	modified:   BiliCore/Core/Noticer.cs
```

**Notice: from now on, we'll only provide the release package since the feature #5 would make debug package not load the emojis.**

## 0.1.x - indev versions - updating abandoned.

### 0.1.0.2 - for release package. [Usable]

Release package will working now.

Changes to be committed:

```
	modified:   BiliCommenter/BiliCommenter.csproj
	modified:   BiliCommenter/Properties/AssemblyInfo.cs
```
### 0.1.0.1 [Debug package usable]

Bug fixxed:
```
	#1 Can't log-in correctly.
	#2 Multy-thread callback problem which would cause crash.[comment was sent correctly].
```
Changes to be committed:
```
	modified:   BiliCommenter/BiliCommenter.csproj
	modified:   BiliCommenter/MainWindow.xaml.cs
	modified:   BiliCommenter/Properties/AssemblyInfo.cs
```
### 0.1.0.0 -Initial release. [Buggy]

The initial version.

**It works, but may be buggy.**

Changes to be committed:

```
	new file:   BiliCommenter.sln
	new file:   BiliCommenter/App.config
	new file:   BiliCommenter/App.xaml
	new file:   BiliCommenter/App.xaml.cs
	new file:   BiliCommenter/BiliCommenter.csproj
	new file:   BiliCommenter/MainWindow.xaml
	new file:   BiliCommenter/MainWindow.xaml.cs
	new file:   BiliCommenter/Properties/AssemblyInfo.cs
	new file:   BiliCommenter/Properties/Resources.Designer.cs
	new file:   BiliCommenter/Properties/Resources.resx
	new file:   BiliCommenter/Properties/Settings.Designer.cs
	new file:   BiliCommenter/Properties/Settings.settings
	new file:   BiliCommenter/avatar/ic_22.png
	new file:   BiliCommenter/avatar/ic_22_hide.png
	new file:   BiliCommenter/avatar/ic_33.png
	new file:   BiliCommenter/avatar/ic_33_hide.png
	new file:   BiliCommenter/packages.config
	new file:   BiliCore/API/Account.cs
	new file:   BiliCore/API/Auth.cs
	new file:   BiliCore/API/Bangumi.cs
	new file:   BiliCore/API/Comment.cs
	new file:   BiliCore/API/Common.cs
	new file:   BiliCore/API/MD5.cs
	new file:   BiliCore/BiliCore.csproj
	new file:   BiliCore/Core/CommentTask.cs
	new file:   BiliCore/Core/Noticer.cs
	new file:   BiliCore/Models/AuthModels.cs
	new file:   BiliCore/Models/BangumiEpModel.cs
	new file:   BiliCore/Models/BangumiInfo.cs
	new file:   BiliCore/Models/BangumiSeason.cs
	new file:   BiliCore/Models/EmojisModel.cs
	new file:   BiliCore/Models/UserInfoModel.cs

```