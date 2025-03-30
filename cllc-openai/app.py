from flask import Flask, request, jsonify
from openai import OpenAI
import base64
from pdf2image import convert_from_bytes
from io import BytesIO
import os
from flask_cors import CORS
from dotenv import load_dotenv
import json

load_dotenv()
app = Flask(__name__)
CORS(app)
openai_apikey = os.getenv('OPENAI_API_KEY')
client = OpenAI(api_key=openai_apikey)

# Function to convert PDF to images
def pdf_to_images(pdf_data):
    return convert_from_bytes(pdf_data)

# Function to encode images as base64
def encode_image(image):
    buffered = BytesIO()
    image.save(buffered, format="JPEG")
    return base64.b64encode(buffered.getvalue()).decode("utf-8")

# Flask route to handle PDF file uploads
@app.route('/analyze-pdf', methods=['POST'])
def analyze_pdf():
    # Ensure a file is provided
    if 'file' not in request.files:
        return jsonify({"error": "No file part in the request"}), 400
    
    file = request.files['file']
    
    if file.filename == '':
        return jsonify({"error": "No file selected"}), 400

    if not file.filename.endswith('.pdf'):
        return jsonify({"error": "Invalid file type. Only PDFs are accepted."}), 400

    try:
        # Read the uploaded PDF
        pdf_data = file.read()
        
        # Convert PDF to images
        images = pdf_to_images(pdf_data)

        # Iterate through the images and analyze each one
        stamp_detected = False
        occupancy_load = None
        stamp_date = None
        for page_number, image in enumerate(images, start=1):
            print(f"Analyzing page {page_number}...")
            base64_image = encode_image(image)
            
            # Send the image to the OpenAI Vision model
            response = client.chat.completions.create(
                model="gpt-4o-mini",  # Use the Vision-capable model
                messages=[
                    {
                        "role": "user",
                        "content": [
                            {
                                "type": "text",
                                "text": (
                                    f"Analyze page {page_number} of the document for the presence of an approval stamp. "
                                    f"Approval stamps are issued by authorities to indicate the floor plan has been approved. "
                                    f"An approval stamp will always have a date associated with it, and on the floor plan there will be an occupancy load associated with that plan."
                                    f"If multiple occupant loads are present, for example, one for each room, the value of 'occupancy_load' should be the sum of all the loads. "
                                    f"The response must be a valid JSON object with the following properties: "
                                    f"'has_stamp' (true if the stamp is an 'Occupant Load' stamp, false otherwise), "
                                    f"'occupancy_load' (the associated numerical value or null), "
                                    f"'confidence' (a float representing confidence in detection), and "
                                    f"'date' (the associated date formatted as YYYY-MM-DD or null). "
                                    f"If no 'Occupant Load' stamp is detected, return {{'has_stamp': false, 'occupancy_load': null, 'confidence': null, 'date': null}}. "
                                    f"Respond only with raw JSON. Exclude any formatting or additional text. Do not include the usual ```json``` code block."
                                ),
                            },
                            {
                                "type": "image_url",
                                "image_url": {
                                    "url": f"data:image/jpeg;base64,{base64_image}"
                                },
                            },
                        ],
                    }
                ],
                max_tokens=300,
            )
            # Get raw response
            chat_response = response.choices[0].message.content
            print(f"Raw response from ChatGPT: {chat_response}")
            
            # Clean up response to remove triple backticks if present
            try:
                parsed_response = json.loads(chat_response)
                print(parsed_response)
                
                # Update variables if a valid stamp is detected
                if parsed_response.get("has_stamp", False):
                    stamp_detected = True
                    occupancy_load = parsed_response.get("occupancy_load", None)
                    stamp_date = parsed_response.get("date", None)
                    # No need to continue checking further pages once a stamp is found
                    break
            except json.JSONDecodeError as parse_error:
                print(f"Error parsing ChatGPT response on page {page_number}: {parse_error}")
                print(f"Raw response was: {chat_response}")
                return jsonify({"error": f"Invalid JSON response from ChatGPT on page {page_number}"}), 500

        # Return the result as JSON
        return jsonify({
            "stamp_detected": stamp_detected,
            "occupancy_load": occupancy_load,
            "date": stamp_date
        })

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 3080))
    app.run(debug=True, port=port, host='0.0.0.0')
