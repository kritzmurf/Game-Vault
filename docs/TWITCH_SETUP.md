# Twitch / IGDB Setup

## Overview

To seed data from the Twitch game database (IGDB), 
perform the following steps:

  1. Go to https://dev.twitch.tv/console
  2. Log in with a Twitch account (create one if you don't have one)
  3. Click **Register Your Application**
  4. Fill in:
    - **Name**: something like "Game Vault" (just an identifier, users never see it)
    - **OAuth Redirect URLs**: http://localhost (Its required by Twitch but you likely won't use it.)
    - **Category**: choose "Application Integration"
    - **Client Type**: Confidential
  5. Click **Create**
  6. On the next page, click **New Secret** to generate a client secret
  7. Copy your **Client ID** and **Client Secret** into
     your .env. See .env.example for formatting.


