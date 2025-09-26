# Interview Exercise: Implement Landscaping Pricing System

## Background
The landscaping estimation system needs a pricing calculation method. The `CalculateBasePrice` method currently returns 0 and needs to be implemented.

## Current State
The pricing logic is not implemented. The `CalculateBasePrice` method contains only a TODO comment and returns 0.

## Exercise: Implement Pricing System

### Requirements
Implement pricing logic for landscaping projects with the following categories:

#### Project Types:
- **Standard** - Regular landscaping work
- **Custom** - Custom design and implementation  
- **Peak** - High-demand season (spring/summer)
- **OffSeason** - Low-demand season (winter)
- **Rush** - Expedited timeline
- **Emergency** - Same-day/urgent work

#### Duration Tiers:
- **Short-term (≤30 days)**: Quick projects
- **Medium-term (31-90 days)**: Standard projects  
- **Long-term (>90 days)**: Extended projects

#### Pricing Matrix:
| Project Type | Short-term | Medium-term | Long-term |
|--------------|------------|-------------|-----------|
| **Standard** | $3,750 | $2,500 | $1,800 |
| **Custom** | $7,500 | $5,000 | $3,200 |
| **Peak** | $15,000 | $10,000 | $6,400 |
| **OffSeason** | $3,000 | $2,000 | $1,200 |
| **Rush** | $12,000 | $8,000 | $4,800 |
| **Emergency** | $18,000 | $12,000 | $8,000 |

### Constraints
- **Time Limit**: 30 minutes
- **Scope**: Only modify the `CalculateBasePrice` method in `ProjectEstimatesController.cs`
- **No New Files**: Do not create new files or classes
- **No Dependencies**: Do not add new dependencies or interfaces
- **Backward Compatibility**: All existing project types must continue to work

### Success Criteria

- [ ] All project types return correct pricing for all duration ranges
- [ ] Invalid project types return a reasonable default (Standard pricing)
- [ ] No compilation errors
- [ ] Code is readable and maintainable

### Example Test Cases

```csharp
// Standard project type
CalculateBasePrice("Standard", 20)   // Should return 3750
CalculateBasePrice("Standard", 60)   // Should return 2500
CalculateBasePrice("Standard", 120)  // Should return 1800

// Custom project type
CalculateBasePrice("Custom", 20)     // Should return 7500
CalculateBasePrice("Custom", 60)     // Should return 5000
CalculateBasePrice("Custom", 120)    // Should return 3200

// Peak project type
CalculateBasePrice("Peak", 20)       // Should return 15000
CalculateBasePrice("Peak", 60)       // Should return 10000
CalculateBasePrice("Peak", 120)      // Should return 6400

// OffSeason project type
CalculateBasePrice("OffSeason", 20)  // Should return 3000
CalculateBasePrice("OffSeason", 60)  // Should return 2000
CalculateBasePrice("OffSeason", 120) // Should return 1200

// Rush project type
CalculateBasePrice("Rush", 20)       // Should return 12000
CalculateBasePrice("Rush", 60)       // Should return 8000
CalculateBasePrice("Rush", 120)      // Should return 4800

// Emergency project type
CalculateBasePrice("Emergency", 20)  // Should return 18000
CalculateBasePrice("Emergency", 60)  // Should return 12000
CalculateBasePrice("Emergency", 120) // Should return 8000

// Invalid project type (should default to Standard)
CalculateBasePrice("Invalid", 20)    // Should return 3750
```

### What This Tests

- **Problem Solving**: How to add new functionality to existing code
- **Code Reading**: Understanding existing patterns and logic
- **Attention to Detail**: Ensuring all duration ranges are handled
- **Code Quality**: Writing clean, readable code within constraints
- **Testing**: Verifying the solution works correctly

### Time Estimate
- **Target**: 15-30 minutes
- **Focus**: Simple, focused refactoring task
