# Latexparser-chsarp
Latexparser is a tool to read Latex files and convert them to c# objects. It ONLY contains few commands (specified in Command Dictionary).
Latexparser is under development for the MCADTeX Project (Not yet public).
Latexparser will not differentiate between document and non-document commands

# How to Use Latexparser-csharp
1. Download NuGet-Package via Nuget-Manager in Visual Studio. or directly from  [the website](https://www.nuget.org/packages/latexparse-csharp/)
2. Parse Latex File
```cs
LatexTree tree = LatexParser.ParseFile(filepath);
```

## Creating Custom Commands
latexparse-csharp allows you to provide your own CommandDictionary.xml
CommandDictionary.xml is a file where all recognized commands are being stored to add your own Command copy the existing file from the github page and provide the LatexParser with the specified path. 

CommandDictionary.xml Syntax:
```xml
<package>
    <Command name="commandname">
        <GP name="RequiredGroupParameter" body="False">
            <CmdEnd cmdcall="EndCommand">
        </GP>
        <OP name="OptionalGroupParameter"/>
        <SCP name="SingleCharacterParameter" Key="*"/>
    </Command>
</package>
```
- package -> The package these commands belong to. For Example graphicx functionality will be in the graphicx package
- Command -> name specifies the name of the Command. Commands can have various types of parameters
    * Parameter types:
        * GP: Parameter is required and normally represented within these parentheses {}. Body Setting enables this Command to be a parent e.g.: Section, begin, etc...
            - CmdEnd specifies the Command that will close this body command
        * OP: Parameter is optional and normally represented within these parentheses []. These Parameters cannot be placed after the required parameters
        * SCP: Parameter is optional. If the  Key is embedded in the Commandcall it will be counted as enabled