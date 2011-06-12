
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE" "-keyfile:Mono.Debugger.Cli.snk"
ASSEMBLY = bin/Debug/sdb-cli.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug/

SDB_CLI_EXE_MDB_SOURCE=bin/Debug/sdb-cli.exe.mdb
SDB_CLI_EXE_MDB=$(BUILD_DIR)/sdb-cli.exe.mdb
SDB_CLI_EXE_CONFIG_SOURCE=App.config
MONO_DEBUGGER_SOFT_DLL_SOURCE=../Dependencies/Mono.Debugger.Soft.dll
MONO_CECIL_DLL_SOURCE=../Dependencies/Mono.Cecil.dll
MONO_DEBUGGING_DLL_SOURCE=../Dependencies/Mono.Debugging.dll
OLDNREFACTORY_DLL_SOURCE=../Dependencies/OldNRefactory.dll
MONO_DEBUGGING_SOFT_DLL_SOURCE=../Dependencies/Mono.Debugging.Soft.dll

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE" "-keyfile:Mono.Debugger.Cli.snk"
ASSEMBLY = bin/Release/sdb-cli.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release/

SDB_CLI_EXE_MDB=
SDB_CLI_EXE_CONFIG_SOURCE=App.config
MONO_DEBUGGER_SOFT_DLL_SOURCE=../Dependencies/Mono.Debugger.Soft.dll
MONO_CECIL_DLL_SOURCE=../Dependencies/Mono.Cecil.dll
MONO_DEBUGGING_DLL_SOURCE=../Dependencies/Mono.Debugging.dll
OLDNREFACTORY_DLL_SOURCE=../Dependencies/OldNRefactory.dll
MONO_DEBUGGING_SOFT_DLL_SOURCE=../Dependencies/Mono.Debugging.Soft.dll

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(SDB_CLI_EXE_MDB) \
	$(SDB_CLI_EXE_CONFIG) \
	$(MONO_DEBUGGER_SOFT_DLL) \
	$(MONO_CECIL_DLL) \
	$(MONO_DEBUGGING_DLL) \
	$(OLDNREFACTORY_DLL) \
	$(MONO_DEBUGGING_SOFT_DLL)  

BINARIES = \
	$(SDB_CLI)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	CommandArguments.cs \
	CommandLine.cs \
	Commands/BreakpointCommand.cs \
	Commands/ContinueCommand.cs \
	Commands/ExitCommand.cs \
	Commands/HelpCommand.cs \
	Commands/StartCommand.cs \
	Commands/StopCommand.cs \
	Debugging/ExceptionPrinter.cs \
	Debugging/SoftDebugger.cs \
	ICommand.cs \
	Logging/Logger.cs \
	Logging/LoggerProxy.cs \
	Program.cs \
	Properties/AssemblyInfo.cs \
	Commands/BacktraceCommand.cs \
	Commands/FrameCommand.cs \
	Commands/DisassembleCommand.cs \
	CommandArgumentException.cs \
	Commands/LocalsCommand.cs \
	Commands/EvaluationCommand.cs \
	Debugging/SoftDebuggerCliSession.cs \
	Commands/WatchCommand.cs \
	Configuration.cs \
	Commands/FirstChanceCommand.cs \
	Commands/SourceCommand.cs \
	Commands/CurrentDirectoryCommand.cs \
	Commands/PauseCommand.cs \
	Extensions.cs \
	Commands/StepCommand.cs \
	Commands/InitializeCommand.cs \
	Debugging/DebuggerState.cs \
	Debugging/BacktraceInfo.cs \
	Commands/ThreadCommand.cs \
	Addins/AddinManager.cs \
	Commands/CatchpointCommand.cs \
	Commands/DatabaseCommand.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Mono.Debugger.Cli.snk \
	App.config \
	Addins \
	sdb-cli.in 

REFERENCES =  \
	System \
	System.Core \
	System.Configuration \
	System.Xml

DLL_REFERENCES =  \
	../Dependencies/Mono.Debugger.Soft.dll \
	../Dependencies/Mono.Debugging.dll \
	../Dependencies/Mono.Debugging.Soft.dll \
	../Dependencies/Mono.Cecil.dll \
	../Dependencies/OldNRefactory.dll

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

SDB_CLI_EXE_CONFIG = $(BUILD_DIR)/sdb-cli.exe.config
MONO_DEBUGGER_SOFT_DLL = $(BUILD_DIR)/Mono.Debugger.Soft.dll
MONO_CECIL_DLL = $(BUILD_DIR)/Mono.Cecil.dll
MONO_DEBUGGING_DLL = $(BUILD_DIR)/Mono.Debugging.dll
OLDNREFACTORY_DLL = $(BUILD_DIR)/OldNRefactory.dll
MONO_DEBUGGING_SOFT_DLL = $(BUILD_DIR)/Mono.Debugging.Soft.dll
SDB_CLI = $(BUILD_DIR)/sdb-cli

$(eval $(call emit-deploy-target,SDB_CLI_EXE_CONFIG))
$(eval $(call emit-deploy-target,MONO_DEBUGGER_SOFT_DLL))
$(eval $(call emit-deploy-target,MONO_CECIL_DLL))
$(eval $(call emit-deploy-target,MONO_DEBUGGING_DLL))
$(eval $(call emit-deploy-target,OLDNREFACTORY_DLL))
$(eval $(call emit-deploy-target,MONO_DEBUGGING_SOFT_DLL))
$(eval $(call emit-deploy-wrapper,SDB_CLI,sdb-cli,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
