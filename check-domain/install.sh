#!/bin/bash

curl  -H "Cache-Control: no-cache"  -o "check_b2cdomain.py" "https://raw.githubusercontent.com/razi-rais/Scarps/master/check-domain/check_b2cdomain.py" && curl  -H "Cache-Control: no-cache"  -o "requirements.txt" "https://raw.githubusercontent.com/razi-rais/Scarps/master/check-domain/requirements.txt" && pip install -r  requirements.txt
