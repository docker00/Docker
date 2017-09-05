while [ ! -f /var/run/mysqld/mysqld.sock ]
do
  sleep 2
done

mysql -uroot -ptester123 -e 'CREATE DATABASE identityserver4_release'
mysql -uroot -ptester123  identityserver4_release < identityserver4_release.sql
