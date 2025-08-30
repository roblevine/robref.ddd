---
description: 'Plan the next step of implementation'
tools: ['extensions', 'runTests', 'codebase', 'usages', 'vscodeAPI', 'problems', 'changes', 'testFailure', 'terminalSelection', 'terminalLastCommand', 'openSimpleBrowser', 'fetch', 'findTestFiles', 'searchResults', 'githubRepo', 'runCommands', 'runTasks', 'editFiles', 'runNotebooks', 'search', 'new']
---
# Planning mode instructions
You are in planning mode. Your task is to generate an implementation plan for a new feature or for refactoring existing code.

First - always start by reading copilot-instructions.md and AI-AGENT.md if you haven't already.

DO NOT MAKE ANY EDITS, other than generating a plan and recording it as a plan file.

Each plan is defined in a markdown file in the plans folder. The plan file name should be of the form PLAN-<nnnn>-<title>.md
where <nnnn> is a zero-padded number (e.g. 0001) that uniquely identifies the plan, and is an increment of the highest existing plan number,
and <title> is a short, descriptive title for the plan.

The plan consists of a Markdown document that describes the implementation plan, including the following sections:

* Header: 

    **Status:** <status - initially "PENDING">
    **Started:** <date>

* Overview: A brief description of the feature or refactoring task. This should include scope and boundaries - what is in scope, what is out of scope.
* Requirements: A list of requirements for the feature or refactoring task.
* Architecture and Design: A description of the architecture and design details for the feature or refactoring task. This should include any system design decisions to be made, component decisions etc.
* Implementation Steps: A detailed list of steps to implement the feature or refactoring task. This should be in the form of a set of phases, each with an associated task list.
* Success Criteria and Tests: A description of the success criteria for the feature or refactoring task. A list of tests that need to be implemented to verify the feature or refactoring task.
* Working Area Scratchpad: A place to jot down notes, ideas, and any other information relevant to the implementation plan - this allows us to resume the plan later if we break off.

