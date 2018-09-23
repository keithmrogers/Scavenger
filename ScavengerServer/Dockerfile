FROM ubuntu:14.04.2

RUN apt-get update && \ 
    apt-get install mono-complete curl unzip && \ 
    mkdir /usr/local/orleans && \
    curl https://chgeuer.blob.core.windows.net/public/orleans1_0.zip > /usr/local/orleans/orleans1_0.zip && \
    cd /usr/local/orleans && \
    unzip orleans1_0.zip

CMD [ "mono", "/usr/local/orleans/Binaries/OrleansServer/OrleansHost.exe" ]