#!/bin/bash

# Navigate to your project directory
cd /e/source/EventManagement 

# Add all changes to git
git add .

# Commit the changes
# You can customize the commit message if needed
COMMIT_MESSAGE="Automated commit on $(date)"
git commit -m "$COMMIT_MESSAGE"

# Push the changes to the remote repository
# Replace 'main' with your branch name if different
git push 
