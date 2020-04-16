export PROJECT_NAMESPACE="yuumcs"
export GIT_URI="https://github.com/brianorwhatever/jag-lcrb-carla-public.git"
export GIT_REF="indy-cat-orgbook"

# The templates that should not have their GIT referances(uri and ref) over-ridden
# Templates NOT in this list will have they GIT referances over-ridden
# with the values of GIT_URI and GIT_REF
export -a skip_git_overrides="bc-reg-fdw-build.json backup-build.json"
