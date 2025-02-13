# Web Application Security Checklist

## Registration and User Data Management
- [ yes ] Implement successful saving of member info into the database
- [ yes ] Check for duplicate email addresses and handle appropriately
- [ yes ] Implement strong password requirements:
  - [ yes ] Minimum 12 characters
  - [ yes ] Combination of lowercase, uppercase, numbers, and special characters
  - [ yes ] Provide feedback on password strength
  - [ yes ] Implement both client-side and server-side password checks
- [ yes ] Encrypt sensitive user data in the database (e.g., NRIC, credit card numbers)
- [ yes ] Implement proper password hashing and storage
- [ yes ] Implement file upload restrictions (e.g., .docx, .pdf, or .jpg only)

## Session Management
- [ yes ] reate a secure session upon successful loginC
- [ yes ] Implement session timeout
- [ yes ] Route to homepage/login page after session timeout
- [ yes ] Detect and handle multiple logins from different devices/browser tabs

## Login/Logout Security
- [ yes ] Implement proper login functionality
- [ yes ] Implement rate limiting (e.g., account lockout after 3 failed login attempts)
- [ yes ] Perform proper and safe logout (clear session and redirect to login page)
- [ yes ] Implement audit logging (save user activities in the database)
- [ yes ] Redirect to homepage after successful login, displaying user info

## Anti-Bot Protection
- [ yes ] Implement Google reCAPTCHA v3 service

## Input Validation and Sanitization
- [ yes ] Prevent injection attacks (e.g., SQL injection)
- [ yes ] Implement Cross-Site Request Forgery (CSRF) protection
- [ yes ] Prevent Cross-Site Scripting (XSS) attacks
- [ yes ] Perform proper input sanitization, validation, and verification for all user inputs
- [ yes ] Implement both client-side and server-side input validation
- [ yes ] Display error or warning messages for improper input
- [ yes ] Perform proper encoding before saving data into the database

## Error Handling
- [ ] Implement graceful error handling on all pages
- [ ] Create and display custom error pages (e.g., 404, 403)

## Software Testing and Security Analysis
- [ ] Perform source code analysis using external tools (e.g., GitHub)
- [ ] Address security vulnerabilities identified in the source code

## Advanced Security Features
- [ yes ] Implement automatic account recovery after lockout period
- [ yes ] Enforce password history (avoid password reuse, max 2 password history)
- [ yes ] Implement change password functionality
- [ yes ] Implement reset password functionality (using email link or SMS)
- [ yes ] Enforce minimum and maximum password age policies
- [ yes ] Implement Two-Factor Authentication (2FA)

## General Security Best Practices
- [ yes ] Use HTTPS for all communications
- [ yes ] Implement proper access controls and authorization
- [ ] Keep all software and dependencies up to date
- [ ] Follow secure coding practices
- [ ] Regularly backup and securely store user data
- [ ] Implement logging and monitoring for security events

## Documentation and Reporting
- [ ] Prepare a report on implemented security features
- [ ] Complete and submit the security checklist

Remember to test each security feature thoroughly and ensure they work as expected in your web application.
