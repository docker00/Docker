# Use an official Python runtime as a parent image
FROM python:2.7-slim

# Setting proxy
ENV http_proxy 'http://192.168.4.10:8080'
ENV https_proxy 'https://192.168.4.10:8080'

# Set the working directory to /app
WORKDIR /app

# Copy the current directory contents into the container at /app
ADD providers/python /app

# Install any needed packages specified in requirements.txt
RUN pip install --trusted-host pypi.python.org -r requirements.txt

# Make port 5001 available to the world outside this container
EXPOSE 5001

# Define environment variable
ENV NAME World

# Run app.py when the container launches
CMD ["python", "test.py"]