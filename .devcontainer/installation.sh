#!/bin/sh
echo "starting installation script"

echo "update and install apt packages"
sudo apt update
sudo apt upgrade -y
sudo apt install -y vim iputils-ping dos2unix curl ssh  

echo "checking for SSH public key configuration"
SSH_KEY_FILE=".devcontainer/.ssh-public-key"
if [ -f "$SSH_KEY_FILE" ]; then
    echo "found SSH public key file at $SSH_KEY_FILE"
    echo "setting up Rob's public key for SSH access"
    
    # Ensure .ssh directory exists
    mkdir -p ~/.ssh
    chmod 700 ~/.ssh

    # Add the key from file
    cat "$SSH_KEY_FILE" >> ~/.ssh/authorized_keys
    chmod 600 ~/.ssh/authorized_keys
    
    echo "SSH public key successfully added to authorized_keys"
else
    echo "no SSH public key file found at $SSH_KEY_FILE - skipping SSH key setup"
    echo "to enable SSH access, create $SSH_KEY_FILE with your public key"
fi

echo "install Claude Code"
npm install -g @anthropic-ai/claude-code

#echo "install Cursor CLI"
#curl https://cursor.com/install -fsS | bash

echo "installation script complete"
