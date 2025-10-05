# Interview Exercise: Landscaping Pricing System

## Background
You are working with a landscaping estimation system that calculates pricing for different types of projects. The system has been implemented and is currently in use.

## Exercise: Add On-Demand Work Pricing

### Your Task
Add the ability to save and retrieve pricing for on-demand work and calculate the monthly price for on-demand work to the existing landscaping estimation system. The current implementation has many areas that should be improved, so feel free to refactor if needed to add the new functionality. This should include:

- Implementing the per-service pricing calculation logic for on-demand work
- Ensuring the logic integrates with existing functionality
- Adding appropriate test coverage
- Maintaining code quality and maintainability
- Refactoring existing code if you identify opportunities for improvement (time permitting)

You might need to change the existing data model to accomplish this task.

### Types of Pricing
- Design/Build - design and then build a project for a customer e.g. add a deck to a house (already exists)
- Recurring Service - Perform a recurring task for a customer e.g. Mow the customer's lawn each week (already exists)
- On-Demand - Perform a task when/if its needed e.g. snow removal (new)

### On-Demand Pricing Requirements

#### Pricing Structure
- **Base Rate**: $150 per service for on-demand work
- **Travel Fee**: $25 for locations >15 miles from base

### What You Have Access To
- The complete codebase including controllers, models, and tests
- Existing test cases that demonstrate expected behavior
- The current pricing calculation logic

### Constraints
- **Scope**: Review existing code and add on-demand work as a new pricing model
- **No Breaking Changes**: Maintain backward compatibility
- **Focus**: Code quality, maintainability, and business logic correctness
