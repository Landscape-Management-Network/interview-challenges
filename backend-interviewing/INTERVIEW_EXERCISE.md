# Interview Exercise: Landscaping Pricing System

## Background
You are working with a landscaping estimation system that calculates pricing for different types of projects. The system has been implemented and is currently in use.

## Exercise: Add On-Demand Work Pricing

### Your Task
Add a new pricing model for on-demand work to the existing landscaping estimation system. The current implementation may have some areas that could be improved, so feel free to refactor as needed while adding the new functionality. This should include:

- Implementing the pricing logic for on-demand work
- Ensuring the new model integrates with existing functionality
- Adding appropriate test coverage
- Maintaining code quality and maintainability
- Refactoring existing code if you identify opportunities for improvement

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

### Success Criteria
- Implement on-demand work pricing logic
- Ensure integration with existing pricing models
- Add comprehensive test coverage for the new functionality
- Maintain backward compatibility with existing features
- Consider edge cases and error handling
- Write clean, maintainable code

### What This Tests
- **Feature Implementation**: Ability to add new functionality to existing code
- **Code Integration**: Understanding how to extend existing systems
- **Business Logic**: Designing appropriate pricing models
- **Testing Skills**: Writing comprehensive tests for new features
- **Code Quality**: Writing clean, maintainable code