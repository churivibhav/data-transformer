tableName = "a_table"
log("tableName is set using Python = " + tableName)
sql("drop table "+tableName)
sql("Create table "+tableName+" as select 123 as id, 'abc' as name, current_timestamp as time")
#commit()
data = query("select * from "+tableName)
for row in data: 
    for column in row:
        print(column, " : ",row[column])