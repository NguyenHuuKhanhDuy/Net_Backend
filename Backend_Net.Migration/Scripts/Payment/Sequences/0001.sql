DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'payment') THEN
CREATE SCHEMA payment;
END IF;
END $EF$;