# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a modern web-based course registration system demonstrating a multi-step form flow with tab navigation. The application features a guided process where users must complete each step sequentially before proceeding to the next.

## Architecture

### Core Components

- **CourseRegistrationApp Class** (`js/app.js:2-370`): Main application controller managing state, form validation, and tab navigation
- **Tab Navigation System**: Three-tab interface with progressive access control
- **Form Validation Engine**: Real-time validation with custom error messaging
- **State Management**: Client-side data persistence across tabs

### Tab Flow Structure

1. **流程小助手 (Process Guide)**: Introductory guidance page
2. **學員基本資料 (Basic Information)**: Personal details form (must be completed first)
3. **課程選擇與確認 (Course Selection)**: Course selection and final confirmation (unlocked after step 2)

### Key Features

- Progressive form access control
- Real-time form validation with visual feedback
- Responsive design using CSS Grid and Flexbox
- Modern UI with CSS custom properties for theming
- Client-side state management with data persistence
- Animated transitions and progress tracking

## Development Commands

This is a static web application - no build process required.

### Running the Application

```bash
# Serve locally using any static file server
# Python 3
python -m http.server 8000

# Node.js (if http-server is installed)
npx http-server

# Or simply open index.html in a browser
```

## File Structure

```
auto-flow-example/
├── index.html          # Main HTML structure with all three tabs
├── css/
│   └── style.css       # Modern CSS with custom properties and responsive design
├── js/
│   └── app.js          # Complete application logic and form handling
└── CLAUDE.md           # This documentation file
```

## Key Implementation Details

### Form Validation System
- Real-time validation on field blur events
- Custom validation rules for email and phone formats
- Progressive error display with visual feedback
- Required field validation with contextual error messages

### State Management Pattern
The application uses a centralized state object (`formData`) within the `CourseRegistrationApp` class to maintain form data across tab transitions.

### Tab Access Control
Tab navigation is controlled by the `isBasicInfoCompleted` flag, ensuring users complete the basic information form before accessing course selection.

### CSS Architecture
- CSS custom properties for consistent theming
- Mobile-first responsive design approach
- Component-based styling with BEM-inspired naming
- Modern UI patterns including cards, gradients, and subtle animations

## Customization Notes

- Course types and time slots are configurable in the HTML radio groups and select options
- Color scheme can be modified through CSS custom properties in `:root`
- Form fields can be added/modified in the HTML with corresponding JavaScript validation updates
- Notification system supports success, error, and info message types