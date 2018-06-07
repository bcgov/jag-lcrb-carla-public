# Updating the Dynamics Proxy #

Steps:
1. Login to Dynamics
2. Navigate to <dynamics URL>/api/data/v8.2/$metadata
3. Copy the contents to a file
4. Ensure that the file is valid XML.  For example, the first line should not be blank.
5. Do a search / replace to remove ConcurrencyMode="Fixed"
6. In Visual Studio, ensure that the ODATA v4 template generator is Enabled
7. Ensure that the older Odata Connected Service extension is Disabled (v3 and V4 tools are not compatible)
8. Ensure your .tt file has a valid path to the file you saved in step 3
9. Save the .tt file or "run custom tool" to generate the file
10. Run the application to confirm the new proxy works
 