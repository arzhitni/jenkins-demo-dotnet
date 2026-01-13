FROM alpine:latest
WORKDIR /app
COPY ./publish/ /app/
CMD ["sh", "-lc", "ls -la /app && echo Demo image built OK"]