# This is a Work in Progress

This repository is in active development, as is the documentation.

For now, to run this repository, do the following:

1. Clone and run von-network (https://github.com/bcgov/von-network)

2. Clone and run Indy Catalyst Credential Registry (https://github.com/bcgov/indy-catalyst)

3. Clone and run this repository

- Note, use the ```init.sh``` script to customize your local config, as described [here](#clone-initialize-and-start-your-agent) 

Note that on startup, this repository will:

- Register a public DID on the ledger
- Register schemas and credential definitions on the ledger
- Create a connection to the Indy Cat Cred Registry agent (if necessary)
- Register this issuer with Indy Cat Cred Registry

To submit credentials, use Postman (or similar, based on your local configuration) to submit the following to http://localhost:5000/issue-credential

```
[
    {
        "schema": "ian-registration.ian-ville",
        "version": "1.0.0",
        "attributes": {
            "corp_num": "ABC12345",
            "registration_date": "2018-01-01", 
            "entity_name": "Ima Permit",
            "entity_name_effective": "2018-01-01", 
            "entity_status": "ACT", 
            "entity_status_effective": "2019-01-01",
            "entity_type": "ABC", 
            "registered_jurisdiction": "BC", 
            "addressee": "A Person",
            "address_line_1": "123 Some Street",
            "city": "Victoria",
            "country": "Canada",
            "postal_code": "V1V1V1",
            "province": "BC",
            "effective_date": "2019-01-01",
            "expiry_date": ""
        }
    },
    {
        "schema": "ian-permit.ian-ville",
        "version": "1.0.0",
        "attributes": {
            "permit_id": "MYPERMIT12345",
            "entity_name": "Ima Permit",
            "corp_num": "ABC12345",
            "permit_issued_date": "2018-01-01", 
            "permit_type": "ABC", 
            "permit_status": "OK", 
            "effective_date": "2019-01-01"
        }
    }
]
```

Or, open a browser at http://localhost:5050 and:

- click on "initialization and load tasks" | "von data db init"
- click on "Run"
- then back to home page (left hand nav bar, click on "Overview")
- click on "von data event processor" then click on "Run"
- this will post about 20 random credentials
- then back to home page (left hand nav bar, click on "Overview")
- click on "von data pipeline status" then click on "Run"
- this will display the summary counts

If you navigate to the Credential Registry (http://localhost:8080) you can search for and view the credentials.


# VON Agent Getting Started Tutorial

This Getting Started Guide is to get someone new to VON Issuer/Verifier Agents up and running in about an hour.  We assume that if you are here, you have some background in the goals and purpose of the Verifiable Organizations Network (VON), OrgBook, VON Issuer/Verifier Agents and GreenLight (decentralized workflow).  If any of this is new to you, please learn more at [https://vonx.io](https://vonx.io). On that site, we recommend the overview in the "About" section, and especially, the webinar linked at the top.

## Table of Contents <!-- omit in toc -->

- [VON Agent Getting Started Tutorial](#von-agent-getting-started-tutorial)
  - [Running in your Browser or on Local Machine](#running-in-your-browser-or-on-local-machine)
  - [Prerequisites](#prerequisites)
    - [In Browser](#in-browser)
    - [Local Machine](#local-machine)
  - [VON Network Setup](#von-network-setup)
    - [In Browser](#in-browser-1)
    - [Local Machine](#local-machine-1)
  - [Step 1: Investigating VON](#step-1-investigating-von)
  - [Step 2: Getting Your VON Issuer/Verifier Agent Running](#step-2-getting-your-von-issuerverifier-agent-running)
    - [In Browser](#in-browser-2)
    - [Local Machine](#local-machine-2)
    - [Clone, Initialize and Start Your Agent](#clone-initialize-and-start-your-agent)
  - [Step 3: Reviewing the Configuration Files](#step-3-reviewing-the-configuration-files)
    - [In Browser](#in-browser-3)
    - [Local Machine](#local-machine-3)
  - [Step 4: Issuing a Credential Using GreenLight](#step-4-issuing-a-credential-using-greenlight)
  - [Step 5: Issuing a Credential Using a JSON File](#step-5-issuing-a-credential-using-a-json-file)
  - [Step 6: Customizing Your Credential](#step-6-customizing-your-credential)
    - [Stopping and Restarting Your Agent](#stopping-and-restarting-your-agent)
  - [Step 7: Changing a Proof Request Prerequisite](#step-7-changing-a-proof-request-prerequisite)
  - [Step 8: Adding a Second, Multi-Cardinality Credential](#step-8-adding-a-second-multi-cardinality-credential)
    - [Updating `schemas.yml`](#updating-schemasyml)
    - [Updating `routes.yml`](#updating-routesyml)
    - [Updating `services.yml`](#updating-servicesyml)
    - [Stop and Start the Agent](#stop-and-start-the-agent)
  - [Conclusion](#conclusion)
    - [Next Steps](#next-steps)

## Running in your Browser or on Local Machine

This guide can be run from within a browser, or if you are more technically inclined, you can run it on your local machine using Docker. In the following sections, there are sub-sections for `In Browser` and `Local Machine`, depending on how you want to run the guide. If you are planning on setting up a new VON Issuer/Verifier Agent instance for your organization, we suggest you use the `Local Machine` path.

## Prerequisites

### In Browser

The only prerequisite (other than a browser) is an account with [Docker Hub](https://hub.docker.com). Docker Hub is the "Play Store" for the [Docker](https://docker.com) ecosystem.

### Local Machine

To run this guide on your local machine, you must have the following installed:

* Docker (Community Edition is fine)
  * If you do not already have Docker installed, go to [the Docker installation page](https://docs.docker.com/install/#supported-platforms) and click the link for your platform.
* Docker Compose
  * Instructions for installing docker-compose on a variety of platforms can be found [here](https://docs.docker.com/compose/install/).
* git
  * [This link](https://www.linode.com/docs/development/version-control/how-to-install-git-on-linux-mac-and-windows/) provides installation instructions for Mac, Linux (including if you are running Linux using VirtualBox) and native Windows (without VirtualBox).
* a bash shell
  * bash is the default shell for Mac and Linux.
  * On Windows, the git-bash version of the bash shell is installed with git and it works well. You **must** use bash to run the guide (PowerShell or Cmd will not work).
* curl
  * An optional step in the guide uses the utility `curl`.
  * curl is included on Mac and Linux.
  * Instructions for installing curl on Windows can be found [here](https://stackoverflow.com/questions/9507353/how-do-i-install-and-use-curl-on-windows).

## VON Network Setup

### In Browser

Go to [Play with Docker](https://labs.play-with-docker.com/) and (if necessary) click the login button. *Play With Docker* is operated by Docker to support developers learning to use Docker.

> If you want to learn more about the `Play with Docker` environment, look at the [About](https://training.play-with-docker.com/about/) and the Docker related tutorials at the Docker Labs [Training Site](https://training.play-with-docker.com). It's all great stuff created by the Docker Community. Kudos!

Click the `Start` button to start a Docker sandbox you can use to run the demo, and then click `+Add an Instance` to start a terminal in your browser. You have an instance of a Linux container running and have a bash command line.  We won't need to use the command line until Step 2 of this tutorial.

From time to time in the steps in this guide, we'll ask you to edit files. There are two ways to do that in this environment:

- If you are comfortable with the `vi` editor, you can just use that. If you don't know `vi`, don't try it. It's a little scary.
- Alternatively, there is an `Editor` button near the top of the screen. Click that and you get a list of files in your home directory, and clicking a file will open it in an editor.  You will probably need to expand the editor window to see the file. Make the changes in the editor and click the `Save` button.
  - Don't forget to click the `Save` button.

The following URLs are used in the steps below for the different components:

- The `von-network` URL - [http://greenlight.bcovrin.vonx.io](http://greenlight.bcovrin.vonx.io). You'll see a ledger browser UI showing four nodes up and running (blue circles).
- The `OrgBook` URL  - [https://demo.orgbook.gov.bc.ca](https://demo.orgbook.gov.bc.ca) - You'll see the OrgBook interface with companies/credentials already loaded.
- The `GreenLight` URL - [https://greenlight.orgbook.gov.bc.ca](https://greenlight.orgbook.gov.bc.ca). You'll see the GreenLight interface, with the `Credential` drop down having a list of at least the seven demo credential types, and perhaps (many) more. Typing into the `Credential` text box enables a search of the credentials list.

You can open those sites now or later. They'll be referenced by name (e.g. "The von-network URL...") in the guide steps.

### Local Machine

On a local machine upon which the prerequisites are setup, we will be installing and starting, in order, instances of [von-network](https://github.com/bcgov/von-network), [OrgBook](https://github.com/bcgov/indy-catalyst) and [GreenLight](https://github.com/bcgov/greenlight) (decentralized workflow).

Use the [VON Network Quick Start Guide](https://github.com/bcgov/greenlight/blob/master/docker/VONQuickStartGuide.md) to start the prerequisite instances and verify that they are running.

## Step 1: Investigating VON

If you are new to VON, see the instructions in the respective repos for how to use the running instances of [von-network](https://github.com/bcgov/von-network), [OrgBook](https://github.com/bcgov/indy-catalyst) and [GreenLight](https://github.com/bcgov/greenlight).

Our goal in this guide is to configure a new permit and/or licence VON issuer/verifier agent so that the credential will be available from the `Credential` drop down in GreenLight.

## Step 2: Getting Your VON Issuer/Verifier Agent Running

In this step, we'll get an instance of your new VON issuer/verifier agent running and issuing credentials.

### In Browser

Start in the root folder of your Docker instance&mdash;where you began.

### Local Machine

Use a different shell from the one used to start the three other components. After opening the new shell, start in the folder where you normally put the clones of your GitHub repos.

### Clone, Initialize and Start Your Agent

Clone the repo, and run the initialization script.

```
# Start in the folder with repos (Local Machine) or home directory (In Browser)
$ git clone https://github.com/bcgov/indy-catalyst-issuer-controller
$ cd indy-catalyst-issuer-controller
$ . init.sh  # And follow the prompts
```

The `init.sh` script does a number of things:

- Prompts for some names to use for your basic agent.
- Prompts for whether you are running with Play With Docker or locally and sets some variables accordingly.
- Registers a DID for you on the ledger that you are using.
- Shows you the lines that were changed in the agent configuration files (in [issuer_controller/config](issuer_controller/config)).

The initial agent you  created issues one credential, using the name you gave it, with a handful of claims: permit ID, permit type, etc. That credential depends on the applying organization already having the BC Registries "Registration" credential. Without already having that credential, an applying organization won't be able to get your agent's credential.

To start your agent, run through these steps:

```
cd docker   # Assumes you were already in the root of the cloned repo
./manage build
./manage start
```

After the last command, you will see a stream of logging commands as the agent starts up. The logging should stabilize with a "Completed sync: indy" entry.

When you need to get back to the command line, you can press `CTRL+C` to stop the stream of log commands. Pressing `CTRL+C` does not stop the containers running, it just stops the log from displaying. If you want to get back to seeing the log, you can run the command `./manage logs` from the `indy-catalyst-issuer-controller/docker` folder.

To verify your agent is running:

1. Go to the `agent URL`, where you should see a "404" (not found) error message. That signals the agent is running, but does not respond to that route.
   1. For `In Browser`, click the "5001" link at the top of the screen. That's the path to your agent.
   2. For `Local Machine`, go to [http://localhost:5001](http://localhost:5001).
2. Go to the `GreenLight URL` (In Browser, Local Machine) where in the `Credential` drop down, you should be able to see your agent's credential.
   1. The `Credential` drop down box is a search box, so just type the name of your organization or credential in it.

All good?  Whoohoo!

## Step 3: Reviewing the Configuration Files

Your agent is configured using the YAML files in the `von-x-agent/config` folder in the repo. In the following, we'll take a look at the files in that folder. As you browse these files, you should see the organization and permit names you entered during initialization.

We'll be working with the files as we go through the tutorial. No need to learn all the details about the YAML file right now. When the time comes, documentation for the files can be found in the [VON Agent Configuration Guide](issuer_controller/config/README.md).

### In Browser

Use the built-in Docker editor to view the files in the `indy-catalyst-issuer-controller/issuer_controller/config` folder.

### Local Machine

Press `CTRL+C` (which stops the agent's rolling message log and gets you back to the command line), then

```bash
# Assuming you are currently in the von-agent-template/docker directory
cd ../issuer_controller/config
ls # list files
more schemas.yml # view files - repeat for other .yml files
```

TODO greenlight is not yet implemented for Aries Agent/Indy Catalyst


## Step 4: Issuing a Credential Using GreenLight

Let's use some techniques to trigger your agent to issue a credential to the OrgBook so that you can look at it.  We'll start with the easiest way&mdash;using GreenLight.

Go to the `GreenLight URL` ([In Browser](https://greenlight.orgbook.gov.bc.ca), [Local Machine](http://localhost:5000)) and select your credential as the target credential you want to be issued. Leave the Legal Entity field blank. Click `Begin` and you will see a GreenLight graph with "Registration" as the first credential, and your agent's credential as the second. As you will recall from the GreenLight demo earlier, GreenLight shows credential dependencies and the colour of the credential label indicates its status.

If you are wondering about how GreenLight works the [How To](https://greenlight.orgbook.gov.bc.ca/about) page provides details about the processing, the graph colours and actions that can be taken.

> NOTE: When getting the credential from your agent, you will have to use the browser back arrow to get back to the GreenLight graph screen. This is different from the core GreenLight credentials which go back to the graph automatically.

Go into OrgBook ([In Browser](https://demo.orgbook.gov.bc.ca), [Local Machine](http://localhost:8080)), and search for the name of the organization you used for the credentials, and review that organization's credentials. It should have the first ever credential issued by your agent. Cool!

We will need yet another company registered for the exercise in the next step so repeat the GreenLight process one more time for a new company. And, remember its name because you'll need it again! Stop at the first credential&mdash;Registration. 

Good stuff. You have a working agent that issues a basic credential!

## Step 5: Issuing a Credential Using a JSON File

Now that you have seen how a user can trigger the issuance of a verifiable credential via a web form, let's look at how an application can issue one via an API call. In most production cases, verifiable credentials will be issued from a service's existing backend application. The application will be adjusted so that when a permit or licence is issued or updated, an "issue verifiable credential" API call is made with the data for the verifiable credential passed in a chunk of JSON. Let's see how that API call works.

> **Note:** If you are running this using the "Local Machine" approach, make sure that you have curl installed. At the command line just run "curl" and see if the command is found. If not, see the prerequisites for how you can install it.

Remember that your credential is set up to depend on the BC Registries `Registration` credential. To populate the JSON structure, we need to get some information from an existing registration credential. Use the second one that you created in the previous tutorial step (you remembered the name, right?). Find the name in OrgBook and on its organization screen, find the "Registration ID" and "Legal Name". When we used GreenLight, those fields came from the proof request. In this case, we're not going to do the proof request, so we need to (correctly!) populate them in the JSON for the API call.

The JSON file we're going to submit is in the `von-agent-template/von-x-agent/testdata` folder, called `sample_permit.json`. Edit that file and make the following changes:

- set the value of "corp_num" to the "registration ID" from OrgBook
- set the value of "legal_name" to the "legal name" from OrgBook
- replace in the "schema" value the "my-permit" and "my-organization" strings to the permit and organization names that you are using
- if you want, update any of the other data elements, making sure that you retain the JSON structure

Save your file and then, from the `von-agent-template` folder, execute this command **REPLACING** "my-organization" with the name of your organization:

```
curl -vX POST http://$ENDPOINT_HOST/my-organization/issue-credential -d @von-x-agent/testdata/sample_permit.json --header "Content-Type: application/json"
```

You should see the results from the `curl` command with an HTTP response of `200` (success) and JSON structure with the status, something like this:

```
[{"success": true, "result": "05da7e24-c24b-4162-b201-157d8afe7a04", "served_by": "b2179bc0df16"}]
```

If the `curl` command failed:
  - Check for typos in your JSON structure 
  - Check for typos in the command you submitted (to review run the command `cat von-x-agent/testdata/sample_permit.json`)
  - Check that you are in the `von-agent-template` folder.
  - Did you remember to change "my-organization" to the name of your organization?

Once the `curl` command succeeds and the verifiable credential has been issued, go into OrgBook, find the organization again and verify that the credential was indeed issued.  If you want, update the dates attributes in the JSON and re-submit the curl command again. Each time you do, OrgBook will make sure that:

- the latest issued credential is "active"
- older credentials are tagged as "revoked" and will not be used for creating proofs

> **NOTE**: It is up to the issuer to make sure that the dates make sense. OrgBook has very few business rules and knows nothing about the process an issuer organization uses for issuing or revoking credentials. As such, the business rules for making sure the right verifiable credentials are issued with the right dates must come the (backend) code that issues the permits and licences.

So, we have a working agent, and we can issue credentials using a form or an API.  Time to make some changes to the credential we are issuing.

## Step 6: Customizing Your Credential

Now that you have a working agent, let's make some changes and really make it your own. We'll start simple and make some changes to the attributes (claims) within your credential&mdash;changing the name of an attribute and adding a new one.

- The "permit_type" field is really about what authorization is being asked for or issued, so let's change that field name to "permit_authorization"
- Let's add another field "permit_notes" that is a free form text field that can be used to add any notes/conditions/limitations about the permit.


If you want, you can make other changes, within limits.  You can't change the "corp_num" or "legal_name" fields, since they come from the prerequisite verifiable credential, BC Registries' Registration credential. All others can be renamed and new fields can be added as needed.

Things to remember as you make the changes:

- The files to be edited are in the `von-x-agent/config` folder - `schema.yml`, `routes.yml` and `services.yml`. No need to change `settings.yml`
- If you rename a credential attribute, check in all three files for the name and change it in all three.
- If you add an attribute, you need only add it in `schema.yml` and `routes.yml`. Recall that in `routes.yml` you are defining how to populate the attribute when submitting the credential data via a form. Best choice for this exercise is to add it as a visible form field (for example just below `permit_type`).
- See  `Stopping and Restarting Your Agent`  below to see if you need to change the `version` of your schema. **HINT**: In this exercise, because you are changing the credential schema, you **do** have to bump the version.
- Remember to save your files.

> **NOTE**: If you are using "Play with Docker" and the "Play with Docker" editor, be warned: the editor has a bad habit of deleting characters at the bottom of a file (it's a known problem to that service).  If you have a problem doing this exercise, check for that.

### Stopping and Restarting Your Agent

When you make changes to the configuration, you will need to stop, rebuild and redeploy your agent.  Here's some guidance about doing that:

1. To stop the agent and keep its wallet, go to the agent's docker folder (`von-agent-template/docker`) and run the command `./manage stop`.
2. Once you've made your changes, stopped your agent and are ready to test, run the commands `./manage build` and `./manage start`. You **must** run the `build` command to pick up your configuration changes.
3. If you change any of the defined schema, you will need to bump the `version` of your schema in `schemas.yml` (e.g. from "1.0.0" to "1.0.1").

> **NOTE**: The command `./manage down` both stops the agent AND deletes the wallet storage. You should only do that when running locally and you want to restart everything - von-network, TheOrgBook and GreenLight.

Once you have restarted your agent, run the GreenLight instructions (Step 4) again to issue a new credential. Notice that both the old (1.0.0) and new (1.0.1) versions of the schema are listed in GreenLight. That's to be expected (at least for now, we may change/fix that in the future). Note that when you run locally and restart all the components from scratch, you can go back to version 1.0.0 of the credential if you want.

All good? Great! If not, make sure you carried out all the steps and try again.

## Step 7: Changing a Proof Request Prerequisite

In this step, we are going to change the proof request prerequisites for your credential by adding another credential to the prerequisite list. To do that, we need to edit the `services.yml` file and add a second proof request dependency. In particular, we're going to add the Ministry of Finance `PST Number` credential as a dependency. Feel free to add a different one if you are feeling adventurous. Here's what you have to do in the `services.yml` file:

1. Add `pst_number` after `greenlight_registration` in the `depends_on` config element.
2. Copy the `greenlight_registration` sub-section of yaml, within `proof_requests` (bottom of the file), and paste immediately below so there are two sections in `proof_requests`.
3. Update the fields in the second copy as appropriate for the new prerequisite credential.
   - To get the values for `did`, `name` and `version`, use the ledger browser ([In Browser](http://greenlight.bcovrin.vonx.io), [Local Machine](http://localhost:9000)) to look up the schema (see image below).
      - Click "Domain"
      - Search for "pst" (or whatever credential type you want to use) and find, copy and paste the `From nym`, `Schema Name` and `Schema version` fields, respectively.
   - Ensure that only attributes that are in the selected schema are in the "attributes" section.
      - You can remove the attributes section entirely if you want (that would be used to prove that the holder has the required verifiable credential without requesting to any of the data within the credential).
4. Save the file.
5. Stop, build and restart the agent as per the instructions in the previous step.

![Searching for Schema on Ledger Browser](images/LedgerExplorerSearch.png)

Once you have done that, go to the GreenLight app and select your agent's credential as the one to be acquired. On the workflow screen you'll see a graph of the new requirements.

Once you have acquired the credentials for a newly registered organization, try requesting the credential for an organization that already has an instance of your agent's credential. You might find that while the organization has the desired credential, it doesn't have all of the prerequisite(s). This is as expected, just like a paper-based permit-issuing services changing their business rules on the fly. Existing credentials (probably) should continue to be valid, but new applications must meet the new requirements. Of course, a service changing its rules could instead choose to revoke credentials issued to organizations that don't already have the new prerequisite, asking or requiring that they 'prove' they have the prerequisite credential before being re-authorized. It's all up to the business rules of the service.

## Step 8: Adding a Second, Multi-Cardinality Credential

The final exercise is a big one. Think of it as your final exam. The instructions are pretty high level, we assume you know where the files are, how to edit them and how to make everything align. Remember in YAML, indentation matters and the leading spaces on lines must be spaces, not tabs.

In this step we'll add an entire new credential that is dependent on the credential already issued by your agent. Here's the business scenario:

> Your credential issuer service offers a "multi-location" permit that extends the authority of an existing permit to other business locations.  If an organization can prove it has been issued the first credential from the service, it can get subsequent permits for other named locations by supplying a name and address.

The following are the tasks to be done and notes about the changes to be made to the files in `von-x-agent/config`:

### Updating `schemas.yml`

`schemas.yml` describes the credentials. To add the second credential, copy the existing one (the entire `- name` element of the YAML), paste it as a second instance, and edit the following:

1. Update the `name` and `version` for the new credential.
2. Update the list of attributes, keeping `corp_num`, `permit_id`, `effective_date`, `permit_issued_date`,and `permit_status` and removing the others. Write down the names of the removed attributes as you go.
3. Remove these attributes from `routes.yml` and `services.yml`.
4. Add the name and address attributes: `location_name`, `addressee`, `civic_address`, `city`, `province`, `postal_code` and `country`

> **NOTE**: If you want to check that your YAML format is valid, you can use a tool such as this [online YAML Validator](https://codebeautify.org/yaml-validator). Copy your full YAML file, paste it into the validator and click `Validate`. A good code editor will have YAML validation built-in.

OK, done! Save your work.

### Updating `routes.yml`
  
`routes.yml` describes the form to be displayed for collecting information about the new credential.  Again, we'll do a big copy and paste to get started. Copy everything from the line below `forms` (the entire entry for the first credential) and  paste it immediately below the copied section.  Once done, start making changes:

1. Remove the sub-sections that reference the attributes that you removed from `schemas.yml`.
2. Change the name of the credential, the path, the schema name, page_title, title, description and explanation. It should be fairly obvious what to put for each.
3. Change `id` under `proof_request` to be the name of the existing credential.

  > **NOTE:** Recall that we added the location name and address fields to the schema, and we have to add them here as well. In `routes.yml`, there is a shortcut for adding addresses to a form as you'll see below. 
  
  1. Add the following text at the bottom of the `fields` list. Remember that indenting in YAML matters, so make sure the additions line up with the rest of the elements of the `fields` list and only uses spaces at the start of lines.

``` yaml
      - name: location_name
        label: Location Name
        type: text
        required: true

      - name: address
        label: Mailing Address
        type: address
        required: true
```

1. Remove `permit_id` from the `mapping` section and add it back into the `fields` section. Put it below `corp_num` with the same `type` and `text` values.

Those are a lot of changes, but we're done with that file. Save your work!

### Updating `services.yml`

As we did in the previous files, we'll copy and paste the existing credential to make the new one and edit from there.  In this case, copy the text from the `description` element below `credential_types` down through the end of the last `model: attribute` element and paste all of that below the existing credential.  The edits to be made are the following:

1. Update `description`, `schema` and `issuer_url` for the new credential.
2. Change `greenlight_registration` under `depends_on` to the name of the existing credential. Leave the `credential` and `topic` elements as is.
3. Add the following lines immediately above `mapping:`

```
      cardinality_fields:
        - location_name
```

> `Cardinality` lets OrgBook know that an organization can hold multiple active instances of a credential at the same time. By default, an organization has only a single active instance of a credential, and receipt of a new credential means the previous one is revoked. With `cardinality` set (in this case, to `location_name`), if a credential is issued for an organization with a specific `location_name` that has not been seen before by OrgBook, the credential is assumed to be a new one.  However, if `location_name` is the same as another credential for that organization, OrgBook assumes it's a reissuance and revokes the old credential. See the [VON issuer/verifier agent documentation](von-x-agent/config/README.md) for more details.

1. In the `mapping` section, remove all of the entries for attributes removed from `schemas.yml`.
2. Insert an `address` section (below) immediately above the `- model: attribute` line.

  > **NOTE:** OrgBook understands the concept of addresses, so we want to map credential address attributes to OrgBook's address search fields.

```yaml
        - model: address
          fields:
            addressee:
              input: addressee
              from: claim
            civic_address:
              input: address_line_1
              from: claim
            city:
              input: city
              from: claim
            province:
              input: province
              from: claim
            postal_code:
              input: postal_code
              from: claim
            country:
              input: country
              from: claim
```

Make sure that the indenting is with spaces and the same as the other `model` elements.

The final section to update is the `proof_requests` at the bottom of the file. Once again, we want to copy the existing elements, paste them and edit them:

1. Copy from `greenlight_registration` to the list of attributes and paste it immediately below.
2. Rename the element to the name of the first credential, as you have earlier for other references to proof requests.
3. Look on the ledger for your first credential to get the `did`, `name` and `version` values.

- The same steps done in the previous tutorial step.
- For attributes, list `corp_num` and `permit_id`.

That's it, you should be good to go.  Time to test.

### Stop and Start the Agent

1. Use the process presented early in this tutorial to stop the agent (without deleting its wallet), build it and start it again.
2. Use GreenLight to test that you can issue the new credential via a form. Did you get the correct GreenLight graph?

> **NOTE:** GreenLight doesn't yet support issuing multiple of the same credential for a single organization, so we'll have to use `curl` to test that. We've added a JSON file ([von-x-agent/testdata/sample-location.json](https://github.com/bcgov/von-agent-template/blob/master/von-x-agent/testdata/sample_location.json)) that you can edit and use to issue multiple credentials to the same organization. 

3. Update the fields to the correct values (especially corp_num and permit_id) before running the curl command.  Try issuing multiple credentials with different dates but the same `location_name` value to see how OrgBook handles that situation. 
4. Check in OrgBook to see the results.

## Conclusion

With that, you should have a pretty good idea of how VON issuer/verifier agents are configured and deployed. See the [agent configuration documentation](von-x-agent/config/README.md) for more details about all of the elements of the YAML files.

If you discover any problems in completing this tutorial, please let us know either by submitting an issue to the source GitHub repo, or by updating the files or documentation and submitting a Pull Request.

### Next Steps

If you are going to be deploying a production agent, you have a few things to consider that become possible within a VON issuer/verifier agent:

- What credentials (registrations, permits, licences, authorizations, etc.) does your service issue? Which of these should be issued as verifiable credentials?
- What events in your existing system trigger the (re)issuance of each of those credentials?
- Within the credentials, what attributes would be useful for an organization to have in their wallet when they need to prove they hold the credentials?
- What are the prerequisite credentials for an entity to be issued your service's credentials?
- What changes could be made in your existing process to support verifiable credentials?
  - What steps can be bypassed by receiving a proof that an organization holds prerequisite verifiable claims?
  - What data elements can be pulled from a prerequisite verifiable credential such that they need not be retyped by the process participants?
  - When are credentials issued in your existing process?