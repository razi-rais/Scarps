#!/bin/bash

curl -o "check_b2cdomain.py" "https://raw.githubusercontent.com/razi-rais/Scarps/master/check-domain/check_b2cdomain.py" && curl -o "requirements.txt" "https://raw.githubusercontent.com/razi-rais/Scarps/master/check-domain/requirements.txt" && pip install -r  requirements.txt && python check_b2cdomain.py -custom-domain "accountuat.contosobank.co.uk" -policy "b2c_1_susi" 