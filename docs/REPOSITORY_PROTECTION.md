# Repository Protection

## General Settings

- Default branch: `main`.
- Visibility: public.
- Automatically delete head branches after merge.
- Enable vulnerability alerts and Dependabot alerts.
- Use squash merge by default.

## Branch Protection for `main`

Require:

- Pull request before merging.
- At least one approving review.
- Dismiss stale approvals when new commits are pushed.
- Conversation resolution before merge.
- Status check `validate` before merge.
- Branches to be up to date before merge.
- Linear history.
- Restrict force pushes and branch deletion.

The concrete status-check context should be confirmed from the first GitHub
Actions run after repository creation.
